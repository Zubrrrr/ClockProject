# ClockProject
Проект выполнен на Unity версии 2022.3.33f1


Этот проект представляет собой модульное приложение, отображающее текущее время и дату. Оно разработано с использованием принципов SOLID и Dependency Injection (DI) с   [Zenject](https://github.com/modesttree/Zenject), а анимация реализована с помощью собственной системы на базе [DOTween](https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676).

Вы также можете ознакомиться с WebGL версией проекта по [ссылке](https://zubrrrr.github.io/Clock/) 



# 
![](ClockPreview.gif)

## Цель проекта

Создать гибкую и расширяемую систему управления временем, которая:
- Поддерживает смену часовых поясов и синхронизацию с сервером
- Обновляет интерфейс в реальном времени
- Реализует анимацию стрелок часов, синхронизированную с текущим временем

## Основные компоненты

### TimeManager
Класс `TimeManager` реализует интерфейс `ITimeService` и отвечает за:
- Синхронизацию текущего времени с удалённым сервером
- Поддержку различных часовых поясов с автоматическим расчетом смещения
- Периодическое обновление времени и отправку событий через `EventAggregator`

#### Основные методы:
- **`SetTimezone(string timezoneId)`** — устанавливает часовой пояс
- **`GetCurrentTime()`** — возвращает текущее время с учётом смещения
- **`UpdateTimeFromServer()`** — асинхронно обновляет время с сервера

### TimeTextModule
Компонент `TimeTextModule` отвечает за отображение текущего времени и даты в пользовательском интерфейсе.
- Подписывается на событие `TimeUpdatedEvent` для обновления текста при изменении времени
- Форматирует и выводит время и дату в TextMeshPro

### ClockAnimationModule
Компонент `ClockAnimationModule` анимирует стрелки часов (часовая, минутная, секундная) в соответствии с текущим временем.
- Использует `ClockHandEntry` для управления параметрами анимации каждой стрелки (включая единицы времени и возможность учёта более мелких единиц времени, таких как секунды в минутной стрелке)
- Подписывается на событие `TimeUpdatedEvent`, чтобы синхронизироваться с актуальным временем

## Кастомная система анимации

Проект включает систему анимации на основе `DOTween`, что позволяет удобно управлять анимациями UI и элементов сцены,комбинировать и строить последовательности из анимаций.
Анимации представлены в виде компонентов, что удобно для анимирования небольшого количества объектов на сцене. Однако ничто не мешает создавать на базе системы коллекции анимаций и управлять ими из отдельных скриптов. Например, я добавил `TimeAnimationModule`, который использует `Zenject` и подписывается на событие `TimeUpdatedEvent`, меняя цвет всех элементов в коллекции  _colorAnimations, когда на часах начинается отсчет новой минуты.

Для данного проекта я создал пять базовых анимаций: 
- Цвет (`ColorAnimation`) - для данной анимации реализованно несколько режимов  `Standard` и `Flash`.
- Прозрачность (`FadeAnimation`)
- Позиция (`MoveAnimation`)
- Поворот (`RotateAnimation`)
- Масштаб (`ScaleAnimation`)

Каждый тип анимации наследуется от абстрактного  `BaseAnimation`, который содержит общие настройки: длительность, тип easing, количество повторений, поведение цикла и задержку.
Каждая анимация может быть предварительно просмотрена в редакторе Unity через встроенные кнопки в инспекторе благодаря редакторским скриптам (`BaseAnimationEditor` и `PreviewControllerEditor`).
Благодаря компонентно-ориентированному подходу мы можем легко изменять и дополнять функционал как уже существующих анимаций, так и создавать новые, не затрагивая “скелет” всей системы. Систему также можно использовать в различных проектах благодаря её гибкости.

## Архитектурные особенности

- **Модульность** — каждый компонент системы отвечает за свою зону ответственности и взаимодействует через события, что позволяет легко добавлять новые модули и менять поведение.
- **Использование DI** — с помощью Zenject все зависимости между классами разрешаются через инъекцию, что упрощает тестирование и расширение функционала.
- **Соблюдение SOLID** — проект построен с учетом принципов SOLID, что делает код более поддерживаемым и гибким.

## Технологии и библиотеки

- **Unity** — основной движок для реализации и запуска проекта.
- **Zenject** — библиотека для внедрения зависимостей (DI), которая упрощает создание модульного и тестируемого кода.
- **DOTween** — используется для реализации анимации в проекте.
