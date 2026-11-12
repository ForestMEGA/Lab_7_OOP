using System;
using System.Collections.Generic;

namespace Lab_4_OOP
{
    class Program
    {
        private static DroneManager manager = new DroneManager();

        private const string CsvFilePath = "drones.csv";
        private const string JsonFilePath = "drones.json";

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            manager.AddDroneDirectly(new Drone(0, "DJI Mavic 3", 5000, 70));
            manager.AddDroneDirectly(new Drone(0, "FPV Bomber", 4500, 130));

            while (true)
            {
                Console.WriteLine("\n МЕНЮ КЕРУВАННЯ ДРОНАМИ");
                Console.WriteLine("1 - Додати об'єкт");
                Console.WriteLine("2 - Переглянути всі об'єкти");
                Console.WriteLine("3 - Знайти об'єкт");
                Console.WriteLine("4 - Продемонструвати поведінку");
                Console.WriteLine("5 - Видалити об'єкт");
                Console.WriteLine("6 - Продемонструвати static-методи");
                Console.WriteLine("7 - Зберегти колекцію об'єктів у файл");
                Console.WriteLine("8 - Зчитати колекцію об'єктів з файлу");
                Console.WriteLine("9 - Очистити колекцію об'єктів");
                Console.WriteLine("0 - Вийти з програми");
                Console.Write("Виберіть дію: ");

                string choice = Console.ReadLine()?.Trim();

                if (choice == "1") CreateDroneMenu();
                else if (choice == "2") ShowAllMenu();
                else if (choice == "3") FindMenu();
                else if (choice == "4") FlightMenu();
                else if (choice == "5") DeleteMenu();
                else if (choice == "6") StaticDemoMenu();
                else if (choice == "7") SaveFileMenu();
                else if (choice == "8") LoadFileMenu();
                else if (choice == "9") ClearCollectionMenu();
                else if (choice == "0") { Console.WriteLine("Вихід з програми..."); break; }
                else Console.WriteLine("Некоректний вибір. Спробуйте ще раз.");
            }
        }
        static void CreateDroneMenu()
        {
            try
            {
                Console.WriteLine("\nВведіть дані дрона (Формат: Модель;Батарея;Швидкість):");
                string inputRow = Console.ReadLine();
                if (Drone.TryParse(inputRow, out Drone parsedDrone))
                {
                    manager.AddDroneDirectly(parsedDrone);
                    Console.WriteLine("[УСПІХ]: Дрон успішно додано.");
                }
                else Console.WriteLine("[ПОМИЛКА]: Некоректний формат рядка.");
            }
            catch (Exception ex) { Console.WriteLine($"[ПОМИЛКА]: {ex.Message}"); }
        }

        static void ShowAllMenu()
        {
            Console.WriteLine($"\nВсього створено об'єктів у класі: {Drone.CorrectlyCreatedObjectsCount}");
            List<Drone> all = manager.GetList();
            if (all.Count == 0) { Console.WriteLine("Список порожній."); return; }
            for (int i = 0; i < all.Count; i++) Console.WriteLine(all[i].GetDetails());
        }

        static void FindMenu()
        {
            Console.WriteLine("Введіть ID дрона для пошуку:");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                Drone found = manager.FindById(id);
                if (found != null) Console.WriteLine(found.GetDetails());
                else Console.WriteLine("Дрон не знайдено.");
            }
        }

        static void FlightMenu()
        {
            try
            {
                Console.WriteLine("Введіть ID дрона:");
                int id = Convert.ToInt32(Console.ReadLine());
                Drone activeDrone = manager.FindById(id);
                if (activeDrone == null) { Console.WriteLine("Дрон не знайдений!"); return; }

                Console.WriteLine("Введіть дистанцію польоту (км):");
                double dist = Convert.ToDouble(Console.ReadLine());
                Console.WriteLine(activeDrone.StartFlight(dist));
            }
            catch (Exception ex) { Console.WriteLine($"Помилка: {ex.Message}"); }
        }

        static void DeleteMenu()
        {
            Console.WriteLine("Введіть ID дрона для видалення:");
            if (int.TryParse(Console.ReadLine(), out int id) && manager.RemoveDrone(id))
                Console.WriteLine("Об'єкт видалено.");
            else Console.WriteLine("Дрон не знайдено.");
        }

        static void StaticDemoMenu()
        {
            Console.WriteLine($"Країна-виробник: {Drone.GlobalManufacturerCountry}");
            Console.WriteLine($"Готових до вильоту: {manager.GetReadyDronesCount()}");
        }


        static void SaveFileMenu()
        {
            Console.WriteLine("\n ЗБЕРЕГТИ КОЛЕКЦІЮ У ФАЙЛ ");
            Console.WriteLine("1 - Зберегти у файл *.csv");
            Console.WriteLine("2 - Зберегти у файл *.json");
            Console.Write("Ваш вибір: ");
            string subChoice = Console.ReadLine()?.Trim();

            try
            {
                if (subChoice == "1")
                {
                    manager.SaveToCsv(CsvFilePath);
                    Console.WriteLine($" Колекцію успішно збережено у файл {CsvFilePath}!");
                }
                else if (subChoice == "2")
                {
                    manager.SaveToJson(JsonFilePath);
                    Console.WriteLine($" Колекцію успішно збережено у файл {JsonFilePath}!");
                }
                else Console.WriteLine("Невірний вибір підменю.");
            }
            catch (Exception ex) { Console.WriteLine($"[ПОМИЛКА ЗБЕРЕЖЕННЯ]: {ex.Message}"); }
        }

        static void LoadFileMenu()
        {
            Console.WriteLine("\n ЗЧИТАТИ КОЛЕКЦІЮ З ФАЙЛУ");
            Console.WriteLine("1 - Зчитати з файлу *.csv");
            Console.WriteLine("2 - Зчитати з файлу *.json");
            Console.Write("Ваш вибір: ");
            string subChoice = Console.ReadLine()?.Trim();

            try
            {
                if (subChoice == "1")
                {
                    int count = manager.LoadFromCsv(CsvFilePath);
                    Console.WriteLine($" З файлу CSV успішно десеріалізовано та додано об'єктів: {count}");
                }
                else if (subChoice == "2")
                {
                    int count = manager.LoadFromJson(JsonFilePath);
                    Console.WriteLine($" З файлу JSON успішно десеріалізовано та додано об'єктів: {count}");
                }
                else Console.WriteLine("Невірний вибір підменю.");
            }
            catch (Exception ex) { Console.WriteLine($"[ПОМИЛКА ЧИТАННЯ]: {ex.Message}"); }
        }

        static void ClearCollectionMenu()
        {
            manager.ClearCollection();
            Console.WriteLine("\n Колекцію об'єктів повністю очищено!");
        }
    }
}
