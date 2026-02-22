namespace Surveillance.TechTree
{
    public partial class TechTree
    {
        public void FillWithExampleData()
        {
            Nodes = new Node[]
            {
                // ===== Layer 0 =====
                new Node
                {
                    id = 0,
                    name = "Animals",
                },

                // ===== Layer 1 =====
                new Node
                {
                    id = 1,
                    name = "Flying",
                    parentIDs = new[] { 0 }
                },
                new Node
                {
                    id = 2,
                    name = "Walking",
                    parentIDs = new[] { 0 }
                },

                // ===== Layer 2 =====
                new Node
                {
                    id = 3,
                    name = "Duck",
                    parentIDs = new[] { 1, 2 }
                },
                new Node
                {
                    id = 4,
                    name = "Eagle",
                    parentIDs = new[] { 1 }
                },
                new Node
                {
                    id = 5,
                    name = "Dog",
                    parentIDs = new[] { 2 }
                },
                new Node
                {
                    id = 6,
                    name = "Cat",
                    parentIDs = new[] { 2 }
                },
            };

            BuildLookup();
        }

        public void FillWithExampleData2()
        {
            Nodes = new Node[]
            {
                // ===== Layer 0 =====
                new Node
                {
                    id = 0,
                    name = "Animals",
                },

                // ===== Layer 1 =====
                new Node
                {
                    id = 1,
                    name = "Flying",
                    parentIDs = new[] { 0 }
                },
                new Node
                {
                    id = 2,
                    name = "Walking",
                    parentIDs = new[] { 0 }
                },
                new Node
                {
                    id = 3,
                    name = "Swimming",
                    parentIDs = new[] { 0 }
                },

                // ===== Layer 2 =====
                new Node
                {
                    id = 4,
                    name = "Ducks",
                    parentIDs = new[] { 1, 2, 3 }
                },
                new Node
                {
                    id = 5,
                    name = "Dogs",
                    parentIDs = new[] { 2 }
                },
                new Node
                {
                    id = 6,
                    name = "Fish",
                    parentIDs = new[] { 3 }
                },
                new Node
                {
                    id = 7,
                    name = "Platypus",
                    parentIDs = new[] { 2, 3}
                },
            };

            BuildLookup();
        } 
    }
}