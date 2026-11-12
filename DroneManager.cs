using System;
using System.Collections.Generic;
using System.IO; // для роботи з файлами
using System.Text;
using System.Text.Json; // для роботи з json

namespace Lab_4_OOP
{
    public class DroneManager
    {
        private List<Drone> droneList = new List<Drone>();
        private int currentId = 1;

        public int GetNextId() => currentId;

        public void AddDroneDirectly(Drone drone)
        {
            if (drone.Id <= 0)
            {
                drone.SetAssignedId(currentId);
                currentId++;
            }
            droneList.Add(drone);
        }

        public List<Drone> GetList() => droneList;

        public Drone FindById(int id)
        {
            for (int i = 0; i < droneList.Count; i++)
            {
                if (droneList[i].Id == id) return droneList[i];
            }
            return null;
        }

        public bool RemoveDrone(int id)
        {
            Drone target = FindById(id);
            if (target != null) return droneList.Remove(target);
            return false;
        }

        //  очищення всієї колекції об'єктів
        public void ClearCollection()
        {
            droneList.Clear();
            currentId = 1;
        } 

        public int GetReadyDronesCount()
        {
            int count = 0;
            for (int i = 0; i < droneList.Count; i++)
            {
                if (droneList[i].Status == "Готовий до вильоту") count++;
            }
            return count;
        }

        public Drone GetFastestDrone()
        {
            if (droneList.Count == 0) return null;
            Drone fastest = droneList[0];
            for (int i = 1; i < droneList.Count; i++)
            {
                if (droneList[i].MaxSpeed > fastest.MaxSpeed) fastest = droneList[i];
            }
            return fastest;
        }
        // Серіалізація та десеріалізація у формат CSV
        public void SaveToCsv(string filePath)
        {
            List<string> lines = new List<string>();

            lines.Add("Model;BatteryCapacity;MaxSpeed");

            for (int i = 0; i < droneList.Count; i++)
            {
                lines.Add(droneList[i].ToString());
            }

            File.WriteAllLines(filePath, lines, Encoding.UTF8);
        }

        public int LoadFromCsv(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Файл CSV не знайдено!");

            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);
            int addedCount = 0;

            for (int i = 1; i < lines.Length; i++)
            {
                if (Drone.TryParse(lines[i], out Drone parsedDrone))
                {
                    AddDroneDirectly(parsedDrone); 
                    addedCount++;
                }
            }

            return addedCount; 
        }

        // Серіалізація та деасеріалізація у формат JSON

        public void SaveToJson(string filePath)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };

            string jsonString = JsonSerializer.Serialize(droneList, options);
            File.WriteAllText(filePath, jsonString, Encoding.UTF8);
        }

        public int LoadFromJson(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Файл JSON не знайдено!");

            string jsonString = File.ReadAllText(filePath, Encoding.UTF8);

            List<Drone> loadedDrones = JsonSerializer.Deserialize<List<Drone>>(jsonString);

            if (loadedDrones == null) return 0;

            int addedCount = 0;
            for (int i = 0; i < loadedDrones.Count; i++)
            {
                // додаю зчитані об'єкти до мого поточного списку
                AddDroneDirectly(loadedDrones[i]);
                addedCount++;
            }

            return addedCount;
        }
    }
}
