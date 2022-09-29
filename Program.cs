using ListaTeste;
using Spectre.Console;

namespace ListaTeste
{
    public enum EventsCode
    {
        Add,
        Remove
    }

    public delegate void MenuAction();
    public record MenuItem(ConsoleKey Code, string Text, MenuAction Action);
    public record Event<T>(EventsCode type, T value);

    class Bank
    {
        public bool running;
        private double amount;
        private readonly Lista<MenuItem> menuList;
        private readonly Lista<Event<double>> eventsList;

        public Bank()
        {
            menuList = new Lista<MenuItem>()
                .Add(new MenuItem(ConsoleKey.D1, "Adicionar dinheiro", AddMoney))
                .Add(new MenuItem(ConsoleKey.D2, "Remover dinheiro", RemoveMoney))
                .Add(new MenuItem(ConsoleKey.D3, "Mostrar histórico", ShowHistory))
                .Add(new MenuItem(ConsoleKey.D4, "Sair", Finish));

            eventsList = new Lista<Event<double>>();
        }

        public void Start()
        {
            running = true;

            while (running)
            {
                Console.Clear();
                ShowMenu();
                AnsiConsole.Markup("Digite o item do menu: ");

                var key = Console.ReadKey();
                var menuItem = menuList.Find((value, index) => value.Code.Equals(key.Key));
                if (menuItem != null) menuItem.Action();
                else Console.WriteLine();
            }
        }

        private void ShowMenu()
        {
            var textMenu = menuList.Reduce((acc, value, index) => acc += $"[bold gray]{value.Code})[/] {value.Text}\n", "");
            AnsiConsole.Markup(textMenu);
            var color = amount < 0 ? "red" : amount > 0 ? "green" : "blue";
            AnsiConsole.Markup($"Seu saldo é [bold {color}]${amount}[/].\n");
        }

        private void AddMoney()
        {
            Console.Clear();
            var addAmount = AnsiConsole.Ask<double>("Digite o valor: ");
            amount += addAmount;
            eventsList.Add(new Event<double>(EventsCode.Add, addAmount));
        }

        private void RemoveMoney()
        {
            Console.Clear();
            var removeAmount = AnsiConsole.Ask<double>("Digite o valor: ");
            amount -= removeAmount;
            eventsList.Add(new Event<double>(EventsCode.Remove, removeAmount));
        }

        private void ShowHistory()
        {
            var text = eventsList.Reduce((acc, @event, index) =>
            {
                if (@event.type.Equals(EventsCode.Add))
                {
                    acc += $"Adicionou valor de ${@event.value}";
                }

                if (@event.type.Equals(EventsCode.Remove))
                {
                    acc += $"Removeu valor de ${@event.value}";
                }

                return acc += "\n";
            }, "");
            text += "[gray]Aperte qualquer coisa para voltar...[/]";

            Console.Clear();
            AnsiConsole.Markup(text);
            Console.ReadKey();
        }

        private void Finish()
        {
            Console.Clear();
            AnsiConsole.Markup("[bold]Até a próxima![/]");
            running = false;
        }
    }

    class Program
    {
        static void Main()
        {
            var program = new Bank();
            program.Start();
            Console.ReadKey();
        }
    }
}
