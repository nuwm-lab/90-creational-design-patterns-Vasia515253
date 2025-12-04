using System;
using System.Collections.Generic;
using System.Linq;

// Головний простір імен для програми
namespace LabWork
{
    // ====================================================================
    // 1. Product (Продукт): Геометрична Фігура
    // ====================================================================
    namespace FigureProduct
    {
        /// <summary>
        /// Клас, що представляє складний об'єкт, який створюється.
        /// Усі зміни стану можливі лише через внутрішні методи, доступні Будівельнику.
        /// </summary>
        public class GeometricFigure
        {
            // Принцип інкапсуляції: публічні властивості лише для читання.
            public string Type { get; private set; } = "Невизначений";
            public double Size { get; private set; } = 0.0;
            public string Color { get; private set; } = "Білий";
            public List<string> Components { get; } = new List<string>();

            // Приватні методи, доступ до яких повинен мати лише будівельник.
            // Вони дозволяють "збирати" фігуру поетапно.
            internal void SetType(string type) => Type = type;
            internal void SetSize(double size) => Size = size;
            internal void SetColor(string color) => Color = color;
            internal void AddComponent(string component) => Components.Add(component);

            public void Display()
            {
                Console.WriteLine($"\n✅ Побудована Фігура:");
                Console.WriteLine($"\t- Тип: {Type}");
                Console.WriteLine($"\t- Колір: {Color}");
                Console.WriteLine($"\t- Розмір (сторона/радіус): {Size:F2}");
                Console.WriteLine($"\t- Додаткові компоненти: {string.Join(", ", Components)}");
            }
        }
    }

    // ====================================================================
    // 2. Builder (Будівельник)
    // ====================================================================
    namespace FigureBuilder
    {
        using FigureProduct; // Дозволяє використовувати GeometricFigure

        /// <summary>
        /// Визначає кроки для створення частин продукту.
        /// </summary>
        public interface IFigureBuilder
        {
            void Reset();
            IFigureBuilder BuildType(string type);
            IFigureBuilder BuildColor(string color);
            IFigureBuilder BuildSize(double size);
            IFigureBuilder AddTexture(string texture);
            // Виправлення помилки: Додано метод AddComponent до інтерфейсу
            IFigureBuilder AddComponent(string component); 
            GeometricFigure GetResult();
        }

        /// <summary>
        /// Реалізує кроки IFigureBuilder і надає готовий об'єкт.
        /// </summary>
        public class ConcreteFigureBuilder : IFigureBuilder
        {
            private GeometricFigure _figure = new GeometricFigure();

            public ConcreteFigureBuilder()
            {
                this.Reset();
            }

            public void Reset()
            {
                this._figure = new GeometricFigure();
            }

            public IFigureBuilder BuildType(string type)
            {
                this._figure.SetType(type);
                return this;
            }

            public IFigureBuilder BuildColor(string color)
            {
                this._figure.SetColor(color);
                return this;
            }

            public IFigureBuilder BuildSize(double size)
            {
                // Додано перевірку на неправильні параметри
                if (size <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(size), "Розмір фігури повинен бути додатнім числом.");
                }
                this._figure.SetSize(size);
                return this;
            }

            public IFigureBuilder AddTexture(string texture)
            {
                this._figure.AddComponent($"Текстура ({texture})");
                return this;
            }
            
            // Реалізація AddComponent, доданого до інтерфейсу
            public IFigureBuilder AddComponent(string component)
            {
                this._figure.AddComponent(component);
                return this;
            }

            /// <summary>
            /// Повертає побудований об'єкт і скидає стан будівельника.
            /// </summary>
            public GeometricFigure GetResult()
            {
                GeometricFigure result = this._figure;
                this.Reset(); 
                return result;
            }
        }

        // ====================================================================
        // 3. Director (Директор)
        // ====================================================================
        /// <summary>
        /// Директор, що керує послідовністю кроків будівництва для типових конфігурацій.
        /// </summary>
        public class FigureDirector
        {
            private IFigureBuilder _builder;

            public IFigureBuilder Builder
            {
                set { _builder = value; }
            }

            // Додано перевірку, що Builder встановлено
            private void CheckBuilder()
            {
                if (_builder == null)
                {
                    throw new InvalidOperationException("Будівельник не встановлений у Директорі.");
                }
            }

            // Метод для побудови "типового" кола
            public void BuildSimpleCircle()
            {
                CheckBuilder();
                this._builder.BuildType("Коло").BuildColor("Синій").BuildSize(5.0);
            }

            // Метод для побудови "типового" квадрата з текстурою
            public void BuildTexturedSquare()
            {
                CheckBuilder();
                this._builder.BuildType("Квадрат").BuildColor("Зелений").BuildSize(10.0).AddTexture("Дерево");
            }
        }
    }


    // ====================================================================
    // 4. Client Code (Клієнтський код)
    // ====================================================================
    using FigureBuilder;
    using FigureProduct;

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("## 🏗️ Демонстрація патерну Будівельник (Builder Pattern) - Рефакторинг v2\n");

            // Створюємо екземпляр Конкретного Будівельника
            var builder = new ConcreteFigureBuilder();
            
            // Створюємо екземпляр Директора
            var director = new FigureDirector { Builder = builder };

            // --- A. Створення фігури за допомогою Директора ---
            try
            {
                Console.WriteLine("--- 1. Фігура, побудована Директором (Типове Коло) ---");
                director.BuildSimpleCircle();
                GeometricFigure circle = builder.GetResult();
                circle.Display();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при створенні кола: {ex.Message}");
            }

            // --- Б. Створення фігури без Директора (гнучке налаштування) ---
            try
            {
                Console.WriteLine("\n--- 2. Фігура, побудована Клієнтом (Трикутник з ланцюговим викликом) ---");
                // Тепер AddComponent є частиною інтерфейсу і не викликає помилок
                GeometricFigure triangle = builder
                    .BuildType("Трикутник")
                    .BuildColor("Червоний")
                    .BuildSize(7.5)
                    .AddTexture("Метал")
                    .AddComponent("Контурна лінія (Довільний компонент)") 
                    .GetResult();
                
                triangle.Display();
            }
            catch (Exception ex)
            {
                 Console.WriteLine($"Помилка при створенні трикутника: {ex.Message}");
            }


            // --- В. Тестування валідації (Size <= 0) ---
            try
            {
                Console.WriteLine("\n--- 3. Тест валідації (Розмір = -1) ---");
                builder.BuildSize(-1.0);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine($"❌ Успішно перехоплено помилку валідації: {ex.Message}");
            }
            finally
            {
                // Незважаючи на помилку, ми повинні скинути будівельник
                builder.Reset();
            }
            
            Console.WriteLine("\nПрограма завершила роботу.");
        }
    }
}
