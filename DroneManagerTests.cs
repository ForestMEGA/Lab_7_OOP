using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lab_4_OOP;
using System;
using System.IO; 

namespace Lab5_Tests
{
    [TestClass]
    public class DroneManagerTests
    {
        private DroneManager _manager;

        // підготовка чистого менеджера перед кожним тестом
        [TestInitialize]
        public void Setup()
        {
            _manager = new DroneManager();
        }

        // очищення після кожного тесту
        [TestCleanup]
        public void Teardown()
        {
            _manager = null;
        }


        [TestMethod]
        public void AddDroneDirectly_CorrectObject_ShouldAddToListAndAssignId()
        {
            Drone drone = new Drone(0, "Test Model", 3000, 50);
            _manager.AddDroneDirectly(drone);

            Assert.AreEqual(1, _manager.GetList().Count);
            Assert.AreEqual(1, _manager.GetList()[0].Id); // перевірка автоматичного id
        }

        [TestMethod]
        public void RemoveDrone_ExistingId_ShouldReturnTrueAndRemoveObject()
        {
            Drone drone = new Drone(0, "To Remove", 2000, 40);
            _manager.AddDroneDirectly(drone);

            bool isRemoved = _manager.RemoveDrone(1);

            Assert.IsTrue(isRemoved);
            Assert.AreEqual(0, _manager.GetList().Count);
        }

        [TestMethod]
        public void GetReadyDronesCount_DifferentStatuses_ShouldReturnCorrectCount()
        {
            Drone d1 = new Drone(0, "Drone 1", 3000, 50); // статус за дефолтом готовий
            Drone d2 = new Drone(0, "Drone 2", 4000, 60);
            d2.Status = "Потрібна зарядка"; // штучно міняю статус

            _manager.AddDroneDirectly(d1);
            _manager.AddDroneDirectly(d2);

            int readyCount = _manager.GetReadyDronesCount();

            Assert.AreEqual(1, readyCount);
        }

        [TestMethod]
        public void GetFastestDrone_MultipleDrones_ShouldReturnDroneWithMaxSpeed()
        {
            Drone slowDrone = new Drone(0, "Slow", 2000, 40);
            Drone fastDrone = new Drone(0, "Fast", 5000, 150);

            _manager.AddDroneDirectly(slowDrone);
            _manager.AddDroneDirectly(fastDrone);

            Drone result = _manager.GetFastestDrone();

            Assert.IsNotNull(result);
            Assert.AreEqual("Fast", result.Model);
        }

        [TestMethod]
        public void GetFastestDrone_EmptyList_ShouldReturnNull()
        {
            Drone result = _manager.GetFastestDrone();
            Assert.IsNull(result);
        }

// Серія тестів для перевірки серіалізації та десеріалізації

        [TestMethod]
        public void ClearCollection_NotEmptyList_ShouldMakeListEmpty()
        {
            // перевірка пункту 9 меню (повне очищення списку)
            _manager.AddDroneDirectly(new Drone(0, "Test", 3000, 50));
            _manager.ClearCollection();

            Assert.AreEqual(0, _manager.GetList().Count);
        }

        [TestMethod]
        public void SaveAndLoadCsv_WithCorruptedRows_ShouldLoadOnlyValidObjects()
        {
            string testCsv = "test_drones.csv";

            // створюю тестовий файл із заголовком, двома правильними дронами і двома "битими" рядками
            string[] fileContent = new string[]
            {
                "Model;BatteryCapacity;MaxSpeed", 
                "DJI Air 3;4241;75",              
                "БитийРядокБезДаних",             
                "Mavic;-500;70",                  
                "FPV Pro;3000;120"              
            };
            File.WriteAllLines(testCsv, fileContent);

            int loadedCount = _manager.LoadFromCsv(testCsv);

            // з 4 рядків даних мало завантажитися тільки 2 коректних
            Assert.AreEqual(2, loadedCount);
            Assert.AreEqual(2, _manager.GetList().Count);
            Assert.AreEqual("DJI Air 3", _manager.GetList()[0].Model);

            if (File.Exists(testCsv)) File.Delete(testCsv);
        }

        [TestMethod]
        public void SaveAndLoadJson_ValidCollection_ShouldSerializeAndAppendToExistingList()
        {
            string testJson = "test_drones.json";
            _manager.AddDroneDirectly(new Drone(0, "Json Drone", 4000, 80));

            _manager.SaveToJson(testJson);

            int loadedCount = _manager.LoadFromJson(testJson);

            Assert.AreEqual(1, loadedCount);
            Assert.AreEqual(2, _manager.GetList().Count); 

            // перевірка, чи перейменувалися властивості у самому файлі json
            string rawJson = File.ReadAllText(testJson);
            Assert.IsTrue(rawJson.Contains("drone_model"));
            Assert.IsTrue(rawJson.Contains("battery_mah"));
            Assert.IsFalse(rawJson.Contains("Status")); 

            if (File.Exists(testJson)) File.Delete(testJson);
        }
    }
}
