# BankASystem
Bank A system as WPF project

Приложение позволяет сохранять и считывать личные данные о клиентах банка "А".  Данные считываются и сохраняются с помощью XML файла.
Основные параметры клиентов:
-ФИО;
-Номер телефона;
-Паспортные данные;
-История изменений данных.

Параметры клиента реализованы классом "Client".

История изменений "ChangesHistory" - одно из свойств класса "Client", представляет собой коллекцию типа List<ChangesData>. "ChangesData" - Класс, хранящий в себе:
- Дату и время изменений;
- Тип изменений;
- Сами изменения (Какое на какое значения были заменены) - данный параметр реализован как коллекция типа List<Change>, где Change - структура, хранящая в себе начальное и измененное значения (string, string);
- Кто вносил изменения.

Основное окно - MainWindow.
Неосновное окно - PasswordWindow (появляется как диалоговое, при попытки входа пользователя в аккаунт менеджера). В нем реализован ввод пароля для входа в аккаунт.

Связь Frontend - Backend реализована по паттерну MVVM во "ViewModelBase". 
В нём заключена логика работы кнопок, всех визуальных элементов, логика работы с Backend (с классом "Employee").

Во "ViewModelBase", все функции работы с данными о клиентах выполняются через методы абстрактного класса "Employee" - файл "Employee.cs". 
Этот базовый класс является абстрактным / методы защищены от вызова извне, так как работа с его методами реализуется через создание экземпляров производных от "Employee" классов - "Consultant" и "Manager".

У "Consultant" ограниченный функционал.
Он может:
+ Считывать ФИО и номера телефона клиентов;
+ Изменять номер телефона клиентов;
+ Просматривать историю изменений данных о клиенте;
+ Осуществлять поиск клиентов по базе данных.
Он не может:
- Изменять ФИО и паспортные данные;
- Просматривать паспортные данные;
- Добавлять новых клиентов.

У "Manager" полный функционал.
Помимо функций класса "Consultant", он может:
+ Считывать все данные о клиенте;
+ Менять все данные о клиенте;
+ Добавлять новых клиентов.

Класс "Employee" реализует весь функционал через базовый класс "DataRepository".
"DataRepository" - основной абстрактный класс, представляющий собой непосредственно систему хранения данных со всем необходимым функционалом (статичные методы / свойства):
- Сериализация данных в .xml файл.
- Десериализация данных из .xml файла в коллекцию типа List<Client>
- Хранение актуального списка всех клиентов List<Client> ClientsList. Именно он сериализуется в файл и в него десериализуются данные из файла.
- Добавление нового клиента "AddClient(Client)".
- Удаление клиента "RemoveClient(Client)".
- Изменение данных клиента "ChangeClient(Client, Client)" (Сюда передается изначальное состояние клиента и измененное).
- Проверка на повторения данных при добвалении / изменении клиентов. Номер телефона и паспортные данные не могут повторяться.

29.04.2024: Добавлена возможность открытия / закрытия (не-)депозитных банковских счетов. Функции перевода денег между счетами, начисления процентов на остаток, простмотра истории операций по счёту.

В перспективе: Добавление нескольких аккаунтов "Manager", добавление функции удаления клиента, общие улучшения кода и др.




