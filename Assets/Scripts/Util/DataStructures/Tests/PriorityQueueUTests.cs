using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using cNel.DataStructures; 
namespace Util.Tests
{
    [TestFixture]
    public class PriorityQueueIntTests
    {
        private PriorityQueue<int, int> queue;

        [SetUp]
        public void SetUp()
        {
            queue = new PriorityQueue<int, int>(PriorityQueueType.Min);
        }

        [TestCase(0, 0)]
        [TestCase(1, 1)]
        [TestCase(1, 2)]
        public void BasicPush(int item, int weight)
        {
            queue.Push(item, weight);
            Assert.AreEqual(queue.Peek(), item);
        }

        
        public static IEnumerable<TestCaseData> MinIsTopData
        {
            get
            {
                yield return new TestCaseData(new List<(int,int)> { (1, 1)}).SetName("Basic");
                
                yield return new TestCaseData(new List<(int,int)> { (1, 1), (3,3), (5,5)}).SetName("InOrderInsertion");
                yield return new TestCaseData(new List<(int,int)> { (5,5), (4,4),(1, 1)}).SetName("ReverseOrderInsertion");
            }
        }
        
        
        
        public static IEnumerable<TestCaseData> MinIsTopAfterRemovalData
        {
            get
            {
                yield return new TestCaseData(new List<(int,int)> { (1, 1), (3,3), (5,5)},3).SetName("InOrderInsertion");
                yield return new TestCaseData(new List<(int,int)> { (5,5), (4,4),(1, 1)}, 4).SetName("ReverseOrderInsertion");
            }
        }
        
        [TestCaseSource(nameof(MinIsTopData))]
        public void MinIsTop(List<(int item, int weight)> items)
        {
            foreach (var pair in items)
            {
                queue.Push(pair.item, pair.weight);
            }
            
            Assert.AreEqual(items.OrderBy(x=>x.weight).FirstOrDefault().item, queue.Peek());
        }
        
        
        [TestCaseSource(nameof(MinIsTopAfterRemovalData))]
        public void MinIsTopAfterRemoval(List<(int item, int weight)> items, int expectedItem)
        {
            foreach (var pair in items)
            {
                queue.Push(pair.item, pair.weight);
            }

            queue.Pop();
            Assert.AreEqual(expectedItem, queue.Peek());
        }
    }
}