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

        public virtual float GetExchangePrice(float quantity)
        {
            float exchangePrice = quantity / QuantitySell * Price;

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
        public float Money { get; protected set; }
        protected Dictionary <Product,float> Products = new Dictionary<Product, float>();

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

        public bool TryGetSellingData(out float exchangePrice, out Product product, out float productQuantity)
        {
            Console.Write("Введите имя продукта: ");
            string inputName = Console.ReadLine();

            if (TryGetProduct(inputName, out Product productToFind))
            {
                ProductToSell productForExchange = (ProductToSell)productToFind;
                Console.Write("Введите количество товара в " + productForExchange.Type + ": ");

                if (float.TryParse(Console.ReadLine(), out productQuantity))
                {
                    exchangePrice = productForExchange.GetExchangePrice(productQuantity);

                    if (exchangePrice != 0)
                    {
                        product = productForExchange;

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

            productQuantity = 0;
            product = null;
            exchangePrice = 0;
            return false;
        }

        public bool TrySellProduct(Product product, float quantity)
        {
            if (Products[product] >= quantity)
            {
                return true;
            }
            
            Console.WriteLine("Столько товара нет. Есть только " + Products[product] + " " + product.Type);
            return false;
        }

        public void SellProduct(float exchangePrice, Product sellingProduct, float quantity)
        {
            Money += exchangePrice;
            Products[sellingProduct] -= quantity;
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

        public void BuyProduct(float exchangePrice, Product buyableProduct, float quantity)
        {
            Money -= exchangePrice;
            Product product = new Product(buyableProduct.Name, buyableProduct.Type);

            if (TryGetProduct(product.Name, out Product existingProduct))
            {
                Products[existingProduct]+= quantity;
            }
            else
            {
                Products.Add(product, quantity);
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

    class Exchange
    {
        private Clerk _clerk;
        private Player _player;

        public Exchange(Clerk clerk, Player player)
        {
            _clerk = clerk;
            _player = player;
        }

        public void SellProduct()
        {
            if (_clerk.TryGetSellingData(out float exchangePrice, out Product productForExchange, out float quantity))
            {
                if (_clerk.TrySellProduct(productForExchange, quantity) && _player.TryPayProduct(exchangePrice))
                {
                    _clerk.SellProduct(exchangePrice, productForExchange, quantity);
                    _player.BuyProduct(exchangePrice, productForExchange, quantity);
                }
            }
        }
    }

    class Shop
    {
        private Clerk _clerk;
        private Player _player;
        private Exchange _exchange;

        public Shop(Clerk clerk, Player player)
        {
            _clerk = clerk;
            _player = player;

            _exchange = new Exchange(_clerk, _player);
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
                        _exchange.SellProduct();
                        break;

                    case ShowClerkProductsCommand:
                        _clerk.ShowProductsInfo();
                        break;

                    case ShowPlayerProductsCommand:
                        _player.ShowProductsInfo();
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
}
