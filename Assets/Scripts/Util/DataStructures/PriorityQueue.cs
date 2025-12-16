using System;
using System.Collections;
using System.Collections.Generic;

namespace cNel.DataStructures 
{
    
    public enum PriorityQueueType
    {
        Max,
        Min
    }
    public class PriorityQueue<TElement, TPriority>
    {
        private const int MinSize = 64;
        
        private (TElement element, TPriority weight)[] _data;
        private int _size;
        private int _count;
        private PriorityQueueType _type;
        
        public PriorityQueue(PriorityQueueType type, int size = MinSize) {
            if (size <= MinSize) {
                size = (int)MinSize;
            }

            _type = type;
            _size = size;
            _count = 0;
            _data = new (TElement, TPriority)[size];
        }

        public void Push(TElement element, TPriority weight)
        {
            if (_count + 1 > _size) {
                Resize(2);
            }

            if (_count == 0) {
                _data[_count++] = (element, weight);
                return;
            }

            _data[_count++] = (element, weight);
            Swim(_count-1);
        }

        public TElement Peek() {
            if (_count <= 0) {
                throw new ArgumentException("PQ is empty");
            }
            
            return _data[0].element;
        }

        public TElement Pop()
        {
            if (_count == 0) {
                throw new ArgumentException("PQ is empty");
            }

            if (_count == 1) {
                return _data[--_count].element;
            }
            
            TElement val = _data[0].element;
            _data[0] = _data[--_count];
            Sink(0);
            
            if (_count <= _size / 4 && _size > MinSize){
                Resize(0.5f);
            }

            return val;
        }
        
        private void Resize(float factor)
        {
            _size = (int)(_size * factor);
            (TElement, TPriority)[] newData = new (TElement, TPriority)[_size];

            for (int i = 0; i < _count; i++)
            {
                newData[i] = _data[i];
            }

            _data = newData;
        }

        private void Swim(int index)
        {
            if (index >= _count || index <= 0) {
                return;
            }
            
            int parent = (index - 1) / 2;
            switch (_type)
            {
                case PriorityQueueType.Max:
                    while (index != 0 && Comparer<TPriority>.Default.Compare(_data[index].weight, _data[parent].weight) > 0)
                    {
                        (_data[index], _data[parent]) = (_data[parent], _data[index]);
                        index = parent;
                        parent = (index - 1) / 2;
                    }
                    
                    break;
                case PriorityQueueType.Min:
                    
                    while (index != 0 && Comparer<TPriority>.Default.Compare(_data[index].weight, _data[parent].weight) < 0)
                    {
                        (_data[index], _data[parent]) = (_data[parent], _data[index]);
                        index = parent;
                        parent = (index-1) / 2;
                    }
                    
                    break;
            }
        }

        private void Sink(int index)
        {
            if (index >= _count) {
                return;
            }
            
            int leftChild = index * 2 + 1;
            int rightChild = index * 2 + 2;
            var comparer = Comparer<TPriority>.Default;

            int bestChild = leftChild;
            
            switch (_type)
            {
                case PriorityQueueType.Max:

                    while (leftChild < _count)
                    {
                        bestChild = leftChild;
                        if (rightChild < _count)
                        {
                            bestChild = comparer.Compare(_data[rightChild].weight, _data[leftChild].weight) > 0
                                ? rightChild
                                : leftChild; 
                        }

                        if (comparer.Compare(_data[bestChild].weight, _data[index].weight) > 0)
                        {
                            (_data[bestChild], _data[index]) = (_data[index], _data[bestChild]);
                            
                            index = bestChild;
                            leftChild = index * 2 + 1;
                            rightChild = index * 2 + 2;
                            continue;
                        }
                        
                        break;
                    }
                    
                    break;
                case PriorityQueueType.Min:
                    
                    while (leftChild < _count)
                    {
                        bestChild = leftChild;
                        if (rightChild < _count)
                        {
                            bestChild = comparer.Compare(_data[rightChild].weight, _data[leftChild].weight) < 0
                                ? rightChild
                                : leftChild; 
                        }

                        if (comparer.Compare(_data[bestChild].weight, _data[index].weight) < 0)
                        {
                            (_data[bestChild], _data[index]) = (_data[index], _data[bestChild]);
                            
                            index = bestChild;
                            leftChild = index * 2 + 1;
                            rightChild = index * 2 + 2;
                            continue;
                        }
                        
                        break;
                    }
                    
                    break;
            }
            
        }

        public bool IsEmpty()
        {
            return _count <= 0;
        }
    }
}