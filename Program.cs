using System;
using System.Collections.Generic;

namespace LabWork
{
    // ====================================================================
    // 1. Product (Продукт): Геометрична Фігура
    // ====================================================================
    /// <summary>
    /// Клас, що представляє складний об'єкт, який ми створюємо.
    /// </summary>
    public class GeometricFigure
    {
        public string Type { get; private set; } = "Невизначений";
        public double Size { get; private set; } = 0.0;
        public string Color { get; private set; } = "Білий";
        public List<string> Components { get; } = new List<string>();

        // Принцип інкапсуляції: Setters відсутні, зміна параметрів відбувається лише через Builder.
        
        public void SetType(string type) => Type = type;
        public void SetSize(double size) => Size = size;
        public void SetColor(string color) => Color = color;
        public void AddComponent(string component) => Components.Add(component);

        public void Display()
        {
            Console.WriteLine($"\n✅ Побудована Фігура:");
            Console.WriteLine($"\t- Тип: {Type}");
            Console.WriteLine($"\t- Колір: {Color}");
            Console.WriteLine($"\t- Розмір (сторона/радіус): {Size:F2}");
            Console.WriteLine($"\t- Додаткові компоненти: {string.Join(", ", Components)}");
        }
    }

    // ====================================================================
    // 2. Builder (Будівельник): Інтерфейс
    // ====================================================================
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
        GeometricFigure GetResult();
    }

    // ====================================================================
    // 3. ConcreteBuilder (Конкретний Будівельник)
    // ====================================================================
    /// <summary>
    /// Реалізує кроки IFigureBuilder і надає готовий об'єкт.
    /// </summary>
    public class FigureBuilder : IFigureBuilder
    {
        private GeometricFigure _figure = new GeometricFigure();

        public FigureBuilder()
        {
            this.Reset();
        }

        public void Reset()
        {
            this._figure = new GeometricFigure();
        }

        // Методи повертають IFigureBuilder для ланцюгового виклику (Fluent Interface)
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
            this._figure.SetSize(size);
            return this;
        }

        public IFigureBuilder AddTexture(string texture)
        {
            this._figure.AddComponent($"Текстура ({texture})");
            return this;
        }

        /// <summary>
        /// Повертає побудований об'єкт і скидає стан будівельника.
        /// </summary>
        public GeometricFigure GetResult()
        {
            GeometricFigure result = this._figure;
            this.Reset(); // Скидаємо будівельник для можливості створення нової фігури
            return result;
        }
    }

    // ====================================================================
    // 4. Director (Директор) - необов'язковий, але корисний
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

        // Метод для побудови "типового" кола
        public void BuildSimpleCircle()
        {
            this._builder.BuildType("Коло").BuildColor("Синій").BuildSize(5.0);
        }

        // Метод для побудови "типового" квадрата з текстурою
        public void BuildTexturedSquare()
        {
            this._builder.BuildType("Квадрат").BuildColor("Зелений").BuildSize(10.0).AddTexture("Дерево");
        }
    }


    // ====================================================================
    // 5. Client Code (Клієнтський код)
    // ====================================================================
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("## 🏗️ Демонстрація патерну Будівельник (Builder Pattern)\n");

            // Створюємо екземпляр Конкретного Будівельника
            var builder = new FigureBuilder();
            
            // Створюємо екземпляр Директора
            var director = new FigureDirector { Builder = builder };

            // --- A. Створення фігури за допомогою Директора (типові конфігурації) ---
            
            Console.WriteLine("--- 1. Фігура, побудована Директором (Типове Коло) ---");
            director.BuildSimpleCircle();
            GeometricFigure circle = builder.GetResult();
            circle.Display();

            Console.WriteLine("\n--- 2. Фігура, побудована Директором (Текстурований Квадрат) ---");
            director.BuildTexturedSquare();
            GeometricFigure square = builder.GetResult();
            square.Display();


            // --- Б. Створення фігури без Директора (гнучке налаштування) ---

            Console.WriteLine("\n--- 3. Фігура, побудована Клієнтом (Трикутник з ланцюговим викликом) ---");
            GeometricFigure triangle = builder
                .BuildType("Трикутник")
                .BuildColor("Червоний")
                .BuildSize(7.5)
                .AddTexture("Метал")
                .AddComponent("Контурна лінія") // Додавання власного компоненту
                .GetResult();
            
            triangle.Display();

            Console.WriteLine("\nПрограма завершила роботу.");
        }
    }
}
