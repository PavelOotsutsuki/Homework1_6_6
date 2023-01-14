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
            const string CommandCaseExchangeProducts = "1";
            const string CommandCaseShowProductsInfoClerk = "2";
            const string CommandCaseShowProductsInfoPlayer = "3";
            const string CommandCaseExit = "4";

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
                Console.WriteLine(CommandCaseExchangeProducts + ". Совершить покупку");
                Console.WriteLine(CommandCaseShowProductsInfoClerk + ". Просмотреть список доступных товаров");
                Console.WriteLine(CommandCaseShowProductsInfoPlayer + ". Посмотреть свою сумку");
                Console.WriteLine(CommandCaseExit + ". Уйти из магазина");

                Console.WriteLine("Выберите команду: ");
                string command = Console.ReadLine();

                switch(command)
                {
                    case CommandCaseExchangeProducts:
                        exchange.ExchangeProducts();
                        break;
                    case CommandCaseShowProductsInfoClerk:
                        clerk.ShowProductsInfo();
                        break;
                    case CommandCaseShowProductsInfoPlayer:
                        player.ShowProductsInfo();
                        break;
                    case CommandCaseExit:
                        isWork = false;
                        break;
                    default:
                        Console.WriteLine("Неверная команда");
                        break;
                }

                Console.ReadKey();
            }
        }

        class Product
        {
            public string Name { get; protected set; }
            public float QuantityAll { get; protected set; }
            public string Type { get; protected set; }

            public Product (string name, float quantityAll, string type)
            {
                Name = name;
                QuantityAll=quantityAll;
                Type = type;
            }

            public virtual void ShowProductInfo()
            {
                Console.WriteLine(Name + ":");
                Console.WriteLine();
                Console.WriteLine("Количество: " + QuantityAll + " " + Type);
            }

            public void IncreaseQuantity(float increasedQuantity)
            {
                QuantityAll += increasedQuantity;
            }
        }

        abstract class ProductToSell: Product
        {
            protected float Price;
            protected float QuantitySell;

            public ProductToSell(string name, float price, float quantityAll, float quantitySell, string type): base(name,quantityAll,type)
            {
                if (Price >= 0 && QuantityAll >= 0 && QuantitySell >= 0)
                {
                    Price = price;
                    QuantitySell = quantitySell;
                }
            }

            public virtual float Selling(float quantity)
            {
                float exchangePrice = 0;

                if (QuantityAll >= quantity)
                {
                    QuantityAll -= quantity;
                    exchangePrice = quantity / QuantitySell * Price;
                }
                else
                {
                   Console.WriteLine("Столько товара нет. Есть только " + QuantityAll + " " + Type +" товара");
                }

                return exchangePrice;
            }

            public override void ShowProductInfo()
            {
                base.ShowProductInfo();
                Console.WriteLine("Цена: " + Price + " руб за " + QuantitySell + " " + Type);
            }
        }

        class ProductByWeight: ProductToSell
        {
            const string DefaultType = "кг";
            const string GramType = "г";
            const string QuintalType = "ц";
            const string TonType = "т";

            private Dictionary<string, float> _converts = new Dictionary<string, float>(); 

            public ProductByWeight(string name, float priceKilogram, float weightAllKilogram, float weightSellKilogram) : base(name, priceKilogram, weightAllKilogram, weightSellKilogram, DefaultType)
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
                else if (QuantitySell>= minValueForConvertToQuintal && QuantitySell< maxValueForConvertToQuintal)
                {
                    ConvertType(QuintalType);
                }
                else if (QuantitySell>= minValueForConvertToTon)
                {
                    ConvertType(TonType);
                }
            }

            private void AddAllConverts()
            {
                float convertKilogramToGram = 1000;
                float convertKilogramToQuintal = 0.01f;
                float convertKilogramToTon = 0.001f;

                _converts.Add(GramType, convertKilogramToGram);
                _converts.Add(QuintalType, convertKilogramToQuintal);
                _converts.Add(TonType, convertKilogramToTon);
            }

            private void ConvertType(string type)
            {
                QuantitySell *= _converts[type];
                QuantityAll *= _converts[type];
                Type = type;
            }
        }

        class ProductByCount : ProductToSell
        {
            const string DefaultType = "шт";

            public ProductByCount(string name, float price, int countAll, int countSell=1) : base(name, price, Convert.ToSingle(countAll), Convert.ToSingle(countSell), DefaultType) { }
            
            public override float Selling(float count)
            {
                float fallenExchangePrice = 0;

                if (int.TryParse(Convert.ToString(count), out int number))
                {
                    return base.Selling(count);
                }

                Console.WriteLine("Неверно введено кол-во товара");
                return fallenExchangePrice;
            }
        }

        abstract class HavingProducts
        {
            protected List<Product> Products = new List<Product>();
            public float Money { get; protected set; }

            public HavingProducts(float money)
            {
                Money = money;
            }

            public void ShowProductsInfo()
            {
                for (int i = 0; i < Products.Count; i++)
                {
                    ConsoleColor defaultColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine("\nПродукт " + (i + 1));
                    Console.WriteLine();

                    Console.ForegroundColor = defaultColor;

                    Products[i].ShowProductInfo();
                }
            }

            protected bool TryGetProduct(string name, out Product product)
            {
                for (int i = 0; i < Products.Count; i++)
                {
                    if (Products[i].Name == name)
                    {
                        product = Products[i];
                        return true;
                    }
                }

                product = null;
                return false;
            }
        }

        class Clerk: HavingProducts 
        {
            public Clerk(float money): base(money)
            {
                Products.Add(new ProductByCount("Война и мир", 1100, 10));
                Products.Add(new ProductByWeight("Огурцы длинные", 59, 20.4f, 1));
                Products.Add(new ProductByWeight("Огурцы короткие", 79, 16, 1));
                Products.Add(new ProductByWeight("Грецкие орехи", 99, 10.2f, 0.1f));
                Products.Add(new ProductByCount("Пакеты фасованные", 10, 50));
                Products.Add(new ProductByCount("Бокалы для вина", 1100, 9));
            }

            public bool TrySellProduct(ref float playerMoney, out Product product)
            {
                Console.Write("Введите имя продукта: ");
                string inputName = Console.ReadLine();

                if (TryGetProduct(inputName,out Product productToFind))
                {
                    ProductToSell productForExchange = (ProductToSell)productToFind;
                    Console.Write("Введите количество товара в " + productForExchange.Type + ": ");

                    if (float.TryParse(Console.ReadLine(), out float productQuantity))
                    {
                        float exchangePrice = productForExchange.Selling(productQuantity);

                        if (playerMoney - exchangePrice >= 0)
                        {
                            playerMoney -= exchangePrice;
                            Money += exchangePrice;
                            product = new Product(productForExchange.Name, productQuantity, productForExchange.Type);

                            return true;
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

                product = null;
                return false;
            }
        }

        class Player: HavingProducts
        {
            public Player(float money) : base(money) { }

            public void BuyProduct(float money, Product buyableProduct)
            {
                Money = money;

                if (TryGetProduct(buyableProduct.Name,out Product existingProduct))
                {
                    existingProduct.IncreaseQuantity(buyableProduct.QuantityAll);
                }
                else
                {
                    Products.Add(buyableProduct);
                }
            }
        }

        class Exchange
        {
            private Clerk _clerk;
            private Player _player;

            public Exchange (Clerk clerk, Player player)
            {
                _clerk = clerk;
                _player = player;
            }

            public void ExchangeProducts()
            {
                float playerMoney = _player.Money;

                if(_clerk.TrySellProduct(ref playerMoney, out Product productForExchange))
                {
                    _player.BuyProduct(playerMoney, productForExchange);
                }
            }
        }
    }
}
