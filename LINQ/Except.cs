    // Based on and inspired by this MSDN article
    // https://msdn.microsoft.com/en-us/library/bb336390(v=vs.110).aspx
    public static class ProductComparerDriver
    {
        public static void Go()
        {
            Product[] fruits1 = 
            {
                new Product { Name = "apple", Code = 9, Set = 1 },
                new Product { Name = "orange", Code = 4, Set = 1 },
                new Product { Name = "plum", Code = 4, Set = 1 },
                new Product { Name = "apple", Code = 9, Set = 1 },
                new Product { Name = "lemon", Code = 12, Set = 1 }
            };

            Product[] fruits2 = 
            {                
                new Product { Name = "banana", Code = 11, Set = 2 },
                new Product { Name = "lemon", Code = 14, Set = 2 },
                new Product { Name = "orange", Code = 8, Set = 2 },
                new Product { Name = "lemon", Code = 14, Set = 2 },                
                new Product { Name = "apple", Code = 9, Set = 2  },
                new Product { Name = "pineapple", Code = 12, Set = 2 }
            };

            //Get all the elements from the first array
            //except for the elements from the second array.
            ProductComparer comparer = new ProductComparer();
            comparer.ProductDifference += Comparer_ProductDifference;

            IEnumerable except =
                fruits1.Except(fruits2, comparer);

            foreach (var product in except)
                Console.WriteLine("\r\n**** except: " + product + " ****\r\n");
        }

        private static void Comparer_ProductDifference(object sender, ProductDifferenceEventArgs e)
        {
            Console.WriteLine(e);
        }
    }

    public class Product
    {
        public string Name { get; set; }
        public int Code { get; set; }
        public int Set { get; set; }

        public override string ToString()
        {
            return "(" + Name + "|" + Code + "|" + Set + ")";
        }
    }

    // Custom comparer for the Product class
    public class ProductComparer : IEqualityComparer
    {
        // Products are equal if their names and product numbers are equal.
        public bool Equals(Product x, Product y)
        {
            StringBuilder sb = new StringBuilder();
            Console.WriteLine("Equals? => " + x + " VS " + y);
            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the products' properties are equal.
            bool isEqual = true;
            
            if (x.Code != y.Code)
            {
                isEqual = false;
                sb.Append(x.Code + " != " + y.Code);             
            }
            
            if (x.Name != y.Name)
            {
                isEqual = false;
                sb.Append(" | " + x.Name + " != " + y.Name);
            }
            
            if (!isEqual)
            {
                ProductDifferenceEventArgs e = new ProductDifferenceEventArgs()
                {
                    Product1 = x,
                    Product2 = y,
                    Difference = sb.ToString()
                };
                OnProductDifference(e);
            }

            return isEqual;
        }

        // If Equals() returns true for a pair of objects 
        // then GetHashCode() must return the same value for these objects.
        public int GetHashCode(Product product)
        {
            int hashCode = product.Name.GetHashCode();
            Console.WriteLine("GetHashCode => " + hashCode + " " + product);
            // Force the Equals comparison
            return hashCode;        
        }

        protected void OnProductDifference(ProductDifferenceEventArgs e)
        {
            ProductDifference?.Invoke(this, e);
        }

        public event EventHandler ProductDifference;
    }

    public class ProductDifferenceEventArgs : EventArgs
    {
        public Product Product1 { get; set; }
        public Product Product2 { get; set; }
        public string Difference { get; set; }

        public override string ToString()
        {
            return Product1 + " VS " + Product2 + " => " + "Difference: " + Difference;
        }
    }  
