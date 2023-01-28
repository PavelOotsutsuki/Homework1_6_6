using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Homework1_6_6
{
    class Program
    {
        static void Main(string[] args)
        {
            float defaultMoneyClerk = 0;
            float defaultMoneyPlayer = 100000;
            Clerk clerk = new Clerk(defaultMoneyClerk);
            Player player = new Player(defaultMoneyPlayer);
            Shop shop = new Shop(clerk, player);

            shop.Work();
        }
    }

    class Product
    {
        public Product(string name, string type)
        {
            Name = name;
            Type = type;
        }

        public string Type { get; protected set; }
        public string Name { get; protected set; }

        public virtual void ShowInfo()
        {
            Console.WriteLine(Name + ":");
            Console.WriteLine();
        }
    }

    abstract class ProductToSell : Product
    {
        protected float Price;
        protected float QuantitySell;

        public ProductToSell(string name, float price, float quantitySell, string type) : base(name, type)
        {
            Price = price;
            QuantitySell = quantitySell;
        }

        public virtual float GetExchangePrice(float quantity)
        {
            float exchangePrice = quantity / QuantitySell * Price;

            return exchangePrice;
        }

        public override void ShowInfo()
        {
            base.ShowInfo();
            Console.WriteLine("Цена: " + Price + " руб за " + QuantitySell + " " + Type);
        }

        public abstract float ConvertQuantityAll();
    }

    class ProductByWeight : ProductToSell
    {
        private const string DefaultType = "кг";
        private const string GramType = "г";
        private const string QuintalType = "ц";
        private const string TonType = "т";

        private Dictionary<string, float> _converts = new Dictionary<string, float>();

        public ProductByWeight(string name, float priceKilogram, float weightSellKilogram) : base(name, priceKilogram, weightSellKilogram, DefaultType)
        {
            AddAllConverts();

            int maxValueForConvertToGram = 1;
            int minValueForConvertToGram = 0;
            int minValueForConvertToQuintal = 100;
            int maxValueForConvertToQuintal = 1000;
            int minValueForConvertToTon = 1000;

            if (QuantitySell < maxValueForConvertToGram && QuantitySell > minValueForConvertToGram)
            {
                ConvertType(GramType);
            }
            else if (QuantitySell >= minValueForConvertToQuintal && QuantitySell < maxValueForConvertToQuintal)
            {
                ConvertType(QuintalType);
            }
            else if (QuantitySell >= minValueForConvertToTon)
            {
                ConvertType(TonType);
            }
        }

        public override float ConvertQuantityAll()
        {
            return _converts[Type];
        }

        private void AddAllConverts()
        {
            float convertKilogramToGram = 1000;
            float convertKilogramToQuintal = 0.01f;
            float convertKilogramToTon = 0.001f;
            float convertKilogramToKilogram = 1;

            _converts.Add(GramType, convertKilogramToGram);
            _converts.Add(QuintalType, convertKilogramToQuintal);
            _converts.Add(TonType, convertKilogramToTon);
            _converts.Add(DefaultType, convertKilogramToKilogram);
        }

        private void ConvertType(string type)
        {
            QuantitySell *= _converts[type];
            Type = type;
        }
    }

    class ProductByCount : ProductToSell
    {
        private const string DefaultType = "шт";

        public ProductByCount(string name, float price, int countSell = 1) : base(name, price, Convert.ToSingle(countSell), DefaultType) { }

        public override float GetExchangePrice(float count)
        {
            float fallenExchangePrice = 0;

            if (int.TryParse(Convert.ToString(count), out int number))
            {
                return base.GetExchangePrice(count);
            }

            Console.WriteLine("Неверно введено кол-во товара");
            return fallenExchangePrice;
        }

        public override float ConvertQuantityAll()
        {
            return 1;
        }
    }

    abstract class Person
    {
        protected List <Cell> Products = new List<Cell>();

        public Person(float money)
        {
            Money = money;
        }

        public float Money { get; protected set; }

        public void ShowInfo()
        {
            int productNumber = 1;

            foreach (var cell in Products)
            {
                ConsoleColor defaultColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine("\nПродукт " + productNumber++);
                Console.WriteLine();

                Console.ForegroundColor = defaultColor;

                cell.Product.ShowInfo();
                Console.WriteLine("Количество: " + cell.Quantity + " " + cell.Product.Type); 
            }            
        }

        protected bool TryGetSaleProduct(string name, out Cell desiredCell)
        {
            foreach (var cell in Products)
            {
                if (cell.Product.Name==name)
                {
                    desiredCell = cell;
                    return true;
                }
            }

            desiredCell = null;
            return false;
        }
    }

    class Clerk : Person
    {
        public Clerk(float money) : base(money)
        {
            float defaultQuantityWarAndPeace = 10;
            float defaultQuantityCucumbersLong = 20.4f;
            float defaultQuantityCucumbersShort = 16;
            float defaultQuantityWalnuts = 10.2f;
            float defaultQuantityPackedBags = 50;
            float defaultQuantityWineGlasses = 9;

            Products.Add(new Cell(new ProductByCount("Война и мир", 1100), defaultQuantityWarAndPeace));
            Products.Add(new Cell(new ProductByWeight("Огурцы длинные", 59, 1), defaultQuantityCucumbersLong));
            Products.Add(new Cell(new ProductByWeight("Огурцы короткие", 79, 1), defaultQuantityCucumbersShort));
            Products.Add(new Cell(new ProductByWeight("Грецкие орехи", 99, 0.1f), defaultQuantityWalnuts));
            Products.Add(new Cell(new ProductByCount("Пакеты фасованные", 10), defaultQuantityPackedBags));
            Products.Add(new Cell(new ProductByCount("Бокалы для вина", 1100), defaultQuantityWineGlasses));

            foreach (var cell in Products)
            {
                cell.ConvertQuantity(ConvertQuantityAll(cell.Product));
            }
        }

        public bool TryGetSoldData(out float exchangePrice, out Cell cellProduct)
        {
            Console.Write("Введите имя продукта: ");
            string inputName = Console.ReadLine();

            if (TryGetSaleProduct(inputName, out Cell productToFind))
            {
                ProductToSell productForExchange = (ProductToSell)productToFind.Product;
                Console.Write("Введите количество товара в " + productForExchange.Type + ": ");

                if (float.TryParse(Console.ReadLine(), out float productQuantity))
                {
                    exchangePrice = productForExchange.GetExchangePrice(productQuantity);

                    if (exchangePrice != 0)
                    {
                        cellProduct = new Cell(productForExchange, productQuantity);

                        return true;
                    }
                }
                else
                {
                    Console.WriteLine("Ошибка ввода");
                }
            }
            else
            {
                Console.WriteLine("Такого товара нет");
            }

            cellProduct = null;
            exchangePrice = 0;
            return false;
        }

        public bool TrySaleProduct(Cell cellProduct)
        {
            if (TryGetSaleProduct(cellProduct.Product.Name, out Cell cell))
            {
                if (cell.Quantity >= cellProduct.Quantity)
                {
                    return true;
                }
            }
            
            Console.WriteLine("Столько товара нет. Есть только " + cell.Quantity + " " + cellProduct.Product.Type);
            return false;
        }

        public void SaleProduct(float exchangePrice, Cell cellProduct)
        {
            Money += exchangePrice;

            if (TryGetSaleProduct(cellProduct.Product.Name, out Cell cell))
            {
                cell.SaleProduct(cellProduct.Quantity);
            }
        }

        private float ConvertQuantityAll(Product product)
        {
            ProductToSell productToSell = (ProductToSell)product;

            return productToSell.ConvertQuantityAll();
        }
    }

    class Player : Person
    {
        public Player(float money) : base(money) { }

        public void BuyProduct(float exchangePrice, Cell cell)
        {
            Money -= exchangePrice;

            Product product = new Product(cell.Product.Name, cell.Product.Type);
            Cell cellProduct = new Cell(product, cell.Quantity);

            if (TryGetSaleProduct(cellProduct.Product.Name, out Cell existingProduct))
            {
                existingProduct.BuyProduct(cell.Quantity);
            }
            else
            {
                Products.Add(cellProduct);
            }
        }

        public bool TryPayProduct(float exchangePrice)
        {
            if(Money>= exchangePrice)
            {
                return true;
            }

            Console.WriteLine("Недостаточно денег для оплаты товара. У вас только " + Money + " рублей");
            return false;
        }
    }

    class Shop
    {
        private Clerk _clerk;
        private Player _player;

        public Shop(Clerk clerk, Player player)
        {
            _clerk = clerk;
            _player = player;
        }

        public void Work()
        {
            const string ExchangeProductsCommand = "1";
            const string ShowClerkProductsCommand = "2";
            const string ShowPlayerProductsCommand = "3";
            const string ExitCommand = "4";

            bool isWork = true;

            while (isWork)
            {
                Console.Clear();
                Console.WriteLine("Деньги продавца: " + _clerk.Money);
                Console.WriteLine("Деньги покупателя: " + _player.Money + "\n");
                Console.WriteLine(ExchangeProductsCommand + ". Совершить покупку");
                Console.WriteLine(ShowClerkProductsCommand + ". Просмотреть список доступных товаров");
                Console.WriteLine(ShowPlayerProductsCommand + ". Посмотреть свою сумку");
                Console.WriteLine(ExitCommand + ". Уйти из магазина");

                Console.WriteLine("Выберите команду: ");
                string command = Console.ReadLine();

                switch (command)
                {
                    case ExchangeProductsCommand:
                        SaleProduct();
                        break;

                    case ShowClerkProductsCommand:
                        _clerk.ShowInfo();
                        break;

                    case ShowPlayerProductsCommand:
                        _player.ShowInfo();
                        break;

                    case ExitCommand:
                        isWork = false;
                        break;

                    default:
                        Console.WriteLine("Неверная команда");
                        break;
                }

                Console.ReadKey();
            }
        }

        private void SaleProduct()
        {
            if (_clerk.TryGetSoldData(out float exchangePrice, out Cell cellProduct ))
            {
                if (_clerk.TrySaleProduct(cellProduct) && _player.TryPayProduct(exchangePrice))
                {
                    _clerk.SaleProduct(exchangePrice, cellProduct);
                    _player.BuyProduct(exchangePrice, cellProduct);
                }
            }
        }
    }

    class Cell
    {
        public Cell(Product product, float quantity)
        {
            Product = product;
            Quantity = quantity;
        }

        public Product Product { get; private set; }
        public float Quantity { get; private set; }

        public void SaleProduct(float quantity)
        {
            Quantity -= quantity;
        }

        public void BuyProduct(float quantity)
        {
            Quantity += quantity;
        }

        public void ConvertQuantity(float convertValue)
        {
            Quantity *= convertValue;
        }
    }
}
