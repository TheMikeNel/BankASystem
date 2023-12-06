using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace BankASystem
{
    [Serializable]
    public struct Client
    {
        private string _phoneNumber;
        private string _passport;

        public string FIO { get; set; }

        public string PhoneNumber
        {
            get => _phoneNumber;

            set
            {
                if (value.Replace(" ", "").Length >= 2)
                {
                    _phoneNumber = value.Replace(" ", "");
                }
            }
        }

        public string Passport
        {
            get
            {
                if (_passport.Contains(' '))
                    _passport = _passport.Replace(" ", "");

                if (_passport.Length == 10)
                    return $"{_passport.Substring(0, 4)} {_passport.Substring(4, 6)}";
                
                return _passport;
            }
            set
            {
                if (value.Length >= 10 && value.Length <= 11 && value.Count(x => x == ' ') <= 1)
                    _passport = value;
            }
        }

        public List<ChangesData> ChangesHistory { get; set; }

        public Client(string fio, string phoneNumber, string passport, ChangesData changesData)
        {
            _phoneNumber = "12345678901";
            _passport = "1234567890";
            ChangesHistory = new List<ChangesData>();

            FIO = fio;
            PhoneNumber = phoneNumber;
            Passport = passport;

            if (changesData.TypesOfChanges != string.Empty) ChangesHistory.Add(changesData);
        }

        public Client(string fio, string phoneNumber, string passport) 
            : this(fio, phoneNumber, passport, new ChangesData()) { }

        public void AddChangesData(ChangesData changesData)
        {
            ChangesHistory.Add(changesData);
        }

        public string GetLastChangeString()
        {
            return ChangesHistory.Last().ToString();
        }

        public override string ToString() => $"{FIO}\t {PhoneNumber}\t {Passport}";
    }

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
            return $"{ChangesFrom} <= изменено на => {ChangesTo}";
        }
    }
}
