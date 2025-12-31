using System.Collections.Generic;
using System.Linq;
using JSM.Surveillance.Game;
using Unity.VisualScripting;
using UnityEngine;

namespace JSM.Surveillance
{
    public class FactoryGridSimulation 
    {
        private InputMachineInstance _gridInput;
        private OutputMachineInstance _gridOutput;
        private MachineInstance[] _machines; 
        
        private Source _source;

        public FactoryGridSimulation(IEnumerable<MachineObject> machineObjects, Source source)
        {
            _source = source;
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
                }
            }

            _machines = lookup.Select(x=>x.Value).ToArray();
        }

        public void RunTick()
        {
            ReloadMachineResources();
            foreach (var machine in _machines)
            {
                machine.ProcessTicks();
            }
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        public void ReloadMachineResources()
        {
            FeedAllMachines(_gridInput);
        }

        private void FeedAllMachines(MachineInstance currentMachine)
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

            foreach (var machine in nextMachines)
            {
                FeedAllMachines(machine);
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
    }
}