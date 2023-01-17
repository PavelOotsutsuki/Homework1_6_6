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
            const string ExchangeProductsCommand = "1";
            const string ShowClerkProductsCommand = "2";
            const string ShowPlayerProductsCommand = "3";
            const string ExitCommand = "4";

            float defaultMoneyClerk = 0;
            float defaultMoneyPlayer = 100000;
            Clerk clerk = new Clerk(defaultMoneyClerk);
            Player player = new Player(defaultMoneyPlayer);
            Exchange exchange = new Exchange(clerk, player);

            bool isWork = true;

            while (isWork)
            {
                Console.Clear();
                Console.WriteLine("Деньги продавца: " + clerk.Money);
                Console.WriteLine("Деньги покупателя: " + player.Money+"\n");
                Console.WriteLine(ExchangeProductsCommand + ". Совершить покупку");
                Console.WriteLine(ShowClerkProductsCommand + ". Просмотреть список доступных товаров");
                Console.WriteLine(ShowPlayerProductsCommand + ". Посмотреть свою сумку");
                Console.WriteLine(ExitCommand + ". Уйти из магазина");

                Console.WriteLine("Выберите команду: ");
                string command = Console.ReadLine();

                switch(command)
                {
                    case ExchangeProductsCommand:
                        exchange.ExchangeProducts();
                        break;

                    case ShowClerkProductsCommand:
                        clerk.ShowProductsInfo();
                        break;

                    case ShowPlayerProductsCommand:
                        player.ShowProductsInfo();
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
    }

    class Product
    {
        public string Name { get; protected set; }
        public string Type { get; protected set; }

        public Product(string name, string type)
        {
            Name = name;
            Type = type;
        }

        public virtual void ShowProductInfo()
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

        public virtual float Selling(float quantity, ref float quantityAll)
        {
            float exchangePrice = 0;

            if (quantityAll >= quantity)
            {
                quantityAll -= quantity;
                exchangePrice = quantity / QuantitySell * Price;
            }
            else
            {
                Console.WriteLine("Столько товара нет. Есть только " + quantityAll + " " + Type + " товара");
            }

            return exchangePrice;
        }

        public override void ShowProductInfo()
        {
            base.ShowProductInfo();
            Console.WriteLine("Цена: " + Price + " руб за " + QuantitySell + " " + Type);
        }

        public abstract float ConvertQuantityAll();
    }

    class ProductByWeight : ProductToSell
    {
        const string DefaultType = "кг";
        const string GramType = "г";
        const string QuintalType = "ц";
        const string TonType = "т";

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
        const string DefaultType = "шт";

        public ProductByCount(string name, float price, int countSell = 1) : base(name, price, Convert.ToSingle(countSell), DefaultType) { }

        public override float Selling(float count, ref float quantityAll)
        {
            float fallenExchangePrice = 0;

            if (int.TryParse(Convert.ToString(count), out int number))
            {
                return base.Selling(count, ref quantityAll);
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
        protected Dictionary <Product,float> Products = new Dictionary<Product, float>();
        public float Money { get; protected set; }

        public Person(float money)
        {
            Money = money;
        }

        public void ShowProductsInfo()
        {
            int productNumber = 1;

            foreach (var product in Products)
            {
                ConsoleColor defaultColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine("\nПродукт " + productNumber++);
                Console.WriteLine();

                Console.ForegroundColor = defaultColor;

                product.Key.ShowProductInfo();
                Console.WriteLine("Количество: " + product.Value + " " + product.Key.Type); 
            }            
        }

        protected bool TryGetProduct(string name, out Product desiredProduct)
        {
            foreach (var product in Products)
            {
                if (product.Key.Name==name)
                {
                    desiredProduct = product.Key;
                    return true;
                }
            }

            desiredProduct = null;
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

            Products.Add(new ProductByCount("Война и мир", 1100), defaultQuantityWarAndPeace);
            Products.Add(new ProductByWeight("Огурцы длинные", 59, 1), defaultQuantityCucumbersLong);
            Products.Add(new ProductByWeight("Огурцы короткие", 79, 1), defaultQuantityCucumbersShort);
            Products.Add(new ProductByWeight("Грецкие орехи", 99, 0.1f), defaultQuantityWalnuts);
            Products.Add(new ProductByCount("Пакеты фасованные", 10), defaultQuantityPackedBags);
            Products.Add(new ProductByCount("Бокалы для вина", 1100), defaultQuantityWineGlasses);

            foreach (var product in Products)
            {
                Products[product.Key] *=ConvertQuantityAll(product.Key);
            }
        }

        public bool TrySellProduct(ref float playerMoney, out Product product, out float productQuantity)
        {
            Console.Write("Введите имя продукта: ");
            string inputName = Console.ReadLine();

            if (TryGetProduct(inputName, out Product productToFind))
            {
                ProductToSell productForExchange = (ProductToSell)productToFind;
                float quantityAll = Products[productForExchange];
                Console.Write("Введите количество товара в " + productForExchange.Type + ": ");

                if (float.TryParse(Console.ReadLine(), out productQuantity))
                {
                    float exchangePrice = productForExchange.Selling(productQuantity, ref quantityAll);

                    if (playerMoney - exchangePrice >= 0)
                    {
                        if (exchangePrice != 0)
                        {
                            playerMoney -= exchangePrice;
                            Money += exchangePrice;
                            product = new Product(productForExchange.Name, productForExchange.Type);
                            Products[productForExchange] = quantityAll;

                            return true;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Не хватает денег");
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

            productQuantity = 0;
            product = null;
            return false;
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

        public void BuyProduct(float money, Product buyableProduct, float quantity)
        {
            Money = money;
            float quantityAll = 0;

            if (TryGetProduct(buyableProduct.Name, out Product existingProduct))
            {
                Products[existingProduct]+= quantity;
            }
            else
            {
                Products.Add(buyableProduct, quantity);
            }
        }
    }

    class Exchange
    {
        private Clerk _clerk;
        private Player _player;

        public Exchange(Clerk clerk, Player player)
        {
            _clerk = clerk;
            _player = player;
        }

        public void ExchangeProducts()
        {
            float playerMoney = _player.Money;

            if (_clerk.TrySellProduct(ref playerMoney, out Product productForExchange, out float quantity))
            {
                _player.BuyProduct(playerMoney, productForExchange, quantity);
            }
        }
    }
}
