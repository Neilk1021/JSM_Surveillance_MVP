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
                    ID = 0,
                    Name = "Animals",
                },

                // ===== Layer 1 =====
                new Node
                {
                    ID = 1,
                    Name = "Flying",
                    ParentIDs = new[] { 0 }
                },
                new Node
                {
                    ID = 2,
                    Name = "Walking",
                    ParentIDs = new[] { 0 }
                },

                // ===== Layer 2 =====
                new Node
                {
                    ID = 3,
                    Name = "Duck",
                    ParentIDs = new[] { 1, 2 }
                },
                new Node
                {
                    ID = 4,
                    Name = "Eagle",
                    ParentIDs = new[] { 1 }
                },
                new Node
                {
                    ID = 5,
                    Name = "Dog",
                    ParentIDs = new[] { 2 }
                },
                new Node
                {
                    ID = 6,
                    Name = "Cat",
                    ParentIDs = new[] { 2 }
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
                    ID = 0,
                    Name = "Animals",
                },

                // ===== Layer 1 =====
                new Node
                {
                    ID = 1,
                    Name = "Flying",
                    ParentIDs = new[] { 0 }
                },
                new Node
                {
                    ID = 2,
                    Name = "Walking",
                    ParentIDs = new[] { 0 }
                },
                new Node
                {
                    ID = 3,
                    Name = "Swimming",
                    ParentIDs = new[] { 0 }
                },

                // ===== Layer 2 =====
                new Node
                {
                    ID = 4,
                    Name = "Ducks",
                    ParentIDs = new[] { 1, 2, 3 }
                },
                new Node
                {
                    ID = 5,
                    Name = "Dogs",
                    ParentIDs = new[] { 2 }
                },
                new Node
                {
                    ID = 6,
                    Name = "Fish",
                    ParentIDs = new[] { 3 }
                },
                new Node
                {
                    ID = 7,
                    Name = "Platypus",
                    ParentIDs = new[] { 2, 3}
                },
            };

            BuildLookup();
        } 
    }
}