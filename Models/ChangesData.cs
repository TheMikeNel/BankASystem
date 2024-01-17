using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace BankASystem.Models
{
    /// <summary>
    /// Информация об изменениях. Хранит значения: даты и времени, внесенных изменений, типов изменений, кто вносил.
    /// </summary>
    [Serializable]
    public class ChangesData
    {
        [XmlElement("DateTimeOfChange")]
        public string DateTimeOfChangeString
        {
            get => DateTimeOfChange.ToString("yyyy-MM-dd HH:mm:ss");
            set => DateTimeOfChange = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }

        [XmlIgnore]
        public DateTime DateTimeOfChange { get; private set; }

        public List<Change> Changes { get; set; }
        public string TypesOfChanges { get; set; }
        public string ChangedEmployee { get; set; }

        #region Конструкторы

        public ChangesData(List<Change> changes, string typesOfChanges, string employee)
        {
            Changes = changes;
            TypesOfChanges = typesOfChanges;
            ChangedEmployee = employee;

            DateTimeOfChange = DateTime.Now;
        }

        public ChangesData(List<Change> changes, string typesOfChanges)
            : this(changes, typesOfChanges, null) { }

        public ChangesData(string typesOfChanges, string employee)
            : this(null, typesOfChanges, employee) { }

        public ChangesData(string typesOfChanges)
            : this(typesOfChanges, null) { }

        public ChangesData() : this("Instantiate") { }

        #endregion

        public override string ToString()
        {
            StringBuilder changesLine = new StringBuilder();

            if (Changes != null)
            {
                foreach (Change change in Changes)
                {
                    changesLine.Append(change.ToString() + "\n");
                }
            }

            return $"Дата и время: {DateTimeOfChange}\n" +
                $"Тип изменений: {TypesOfChanges}\n" +
                $"Что изменено: {changesLine}\n" +
                $"Кто вносил: {ChangedEmployee}";
        }
    }

    /// <summary>
    /// Информация об изменении. Хранит предыдущее и измененное значения.
    /// </summary>
    [Serializable]
    public struct Change
    {
        public string ChangesFrom { get; set; }
        public string ChangesTo { get; set; }

        public Change(string from, string to)
        {
            ChangesFrom = from;
            ChangesTo = to;
        }

        public override string ToString()
        {
            return $"{ChangesFrom} - изменено на: {ChangesTo}";
        }
    }
}
