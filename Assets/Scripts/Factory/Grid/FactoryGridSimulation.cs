using System;
using System.Collections.Generic;
using System.Linq;
using JSM.Surveillance.Game;
using Unity.VisualScripting;
using UnityEngine;

namespace JSM.Surveillance
{
    public class FactoryGridSimulation 
    {
        private readonly InputMachineInstance _gridInput;
        private OutputMachineInstance _gridOutput;
        private readonly MachineInstance[] _machines; 
        private readonly Source _source;

        private readonly List<ExternalInputInstance> _externalInputs;
        public event Action<Resource> ResourceMade;
        
        
        public FactoryGridSimulation(IEnumerable<MachineObject> machineObjects, Source source)
        {
            _source = source;
            _externalInputs = new List<ExternalInputInstance>();
            Dictionary<MachineObject, MachineInstance> lookup = machineObjects.ToDictionary(x=>x, x=>x.BuildInstance());

            foreach (var machineObject in lookup)
            {
                foreach (var startPort in machineObject.Key.OutputPorts)
                {
                    if(startPort.ConnectionObject == null) continue;
                    if(startPort.ConnectionObject.StartMachine == null) continue;
                    
                    var endMachine = startPort.ConnectionObject.EndMachine;
                    var connection = new ConnectionInstance(machineObject.Value, lookup[endMachine]);
                    var oP = new ProcessorPort(machineObject.Value, connection, startPort.Type);
                    var iP = new ProcessorPort(lookup[endMachine], connection, startPort.Type == NodeType.Start ? NodeType.End : NodeType.Start);
                    
                    machineObject.Value.AddOutputPort(oP);
                    lookup[endMachine].AddInputPort(iP);
                }

                switch (machineObject.Value)
                {
                    case InputMachineInstance imInstance:
                        _gridInput = imInstance;
                        break;
                    case OutputMachineInstance omInstance:
                        _gridOutput = omInstance;
                        break;
                    case ExternalInputInstance eimInstance:
                        _externalInputs.Add(eimInstance);
                        break;
                }
            }

            _machines = lookup.Select(x=>x.Value).ToArray();
            foreach (var machine in _machines)
            {
                machine.OnResourceProduced += ResourceMade;
            }
        }
        
        ~FactoryGridSimulation()
        {
            foreach (var machine in _machines)
            {
                machine.OnResourceProduced -= ResourceMade;
            }
        }


        public void RunTick(int ticks = 1)
        {
            ReloadMachineResources();
            foreach (var machine in _machines)
            {
                machine.ProcessTicks(ticks);
            }
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        private void ReloadMachineResources()
        {
            var visited = new HashSet<MachineInstance>();
            FeedAllMachines(_gridInput, visited);
            foreach (var externalInput in _externalInputs)
            {
                FeedAllMachines(externalInput, visited);
            }
        }

        private void FeedAllMachines(MachineInstance currentMachine, HashSet<MachineInstance> visited)
        {
            if (currentMachine == null) return;
            if (currentMachine is OutputMachineInstance outputMachine)
            {
                _source.HandleOutputResourceVolume(outputMachine);
                return;
            }

            List<MachineInstance> nextMachines = new List<MachineInstance>();
            var splitAmnt = 0;

            foreach (var port in currentMachine.StartingPorts)
            {
                if (port.Connection == null) continue;
                MachineInstance nextMachine = port.Connection.End;
                nextMachines.Add(nextMachine);
                switch (nextMachine)
                {
                    case ProcessorInstance nextProcessor:
                        if (nextProcessor.Recipe != null &&
                            nextProcessor.Recipe.RequiresInput(currentMachine.Output.resource))
                        {
                            ++splitAmnt;
                        }

                        break;
                    case OutputMachineInstance:
                        ++splitAmnt;
                        break;
                    default:
                        break;
                }
            }
            
            if (currentMachine.Output.amount > 0)
            {
                FeedChildMachines(currentMachine, nextMachines, splitAmnt);
            }

            visited.Add(currentMachine);
            foreach (var machine in nextMachines)
            {
                if(visited.Contains(machine))continue;
                
                FeedAllMachines(machine, visited);
            }
        }

        private void FeedChildMachines(MachineInstance currentMachine, List<MachineInstance> nextMachines, int splitAmnt)
        {
            
            foreach (var machine in nextMachines.TakeWhile(_ => splitAmnt > 0 ))
            {
                switch (machine)
                {
                    case ProcessorInstance processor when processor.Recipe == null ||
                                                        !processor.Recipe.RequiresInput(currentMachine.Output.resource):
                        break;
                    case ProcessorInstance processor:
                        
                        FeedProcessorInputs(currentMachine, processor, splitAmnt);
                        splitAmnt -= 1;
                        break;
                    case OutputMachineInstance output:
                        FeedOutputResources(currentMachine, output, splitAmnt);
                        splitAmnt -= 1;
                        break;
                }
            }
        }

        private void FeedOutputResources(MachineInstance previous, OutputMachineInstance outputMachine, int splitAmnt)
        {
            int amnt = previous.RemoveOutput(previous.Output.amount / splitAmnt);
            outputMachine.AddResource(previous.Output.resource, amnt);
        }

        private void FeedProcessorInputs(MachineInstance  previous, ProcessorInstance current, int splitAmnt)
        {
            int amnt = previous.RemoveOutput(previous.Output.amount / splitAmnt);
            int excess = current.AddInput(previous.Output.resource, amnt);
            previous.AddOutput(previous.Output.resource, excess);
        }

        public Resource GetOutputResourceType()
        {
            return _gridOutput?.EndPorts
                .Select(x => x.Connection?.Start switch
                {
                    ProcessorInstance pO => pO.Recipe.OutputVolume.resource,
                    InputMachineInstance iO     => iO.Source.resource,
                    _                    => null
                })
                .FirstOrDefault(resource => resource != null);
        }

    }
}