using ListaTeste;
using Spectre.Console;

namespace ListaTeste
{
    // Códigos reutilizaveis para evento
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
        // Indica se está no loop ou não
        public bool running;

        // Valor do saldo
        private double amount;

        // Lista de items do menu
        private readonly Lista<MenuItem> menuList;

        // Lista de eventos
        private readonly Lista<Event<double>> eventsList;

        public Bank()
        {
            // Criando a lista do menu e configurando com os MenuItem's
            menuList = new Lista<MenuItem>()
                .Add(new MenuItem(ConsoleKey.D1, "Adicionar dinheiro", () => AddMoney(null)))
                .Add(new MenuItem(ConsoleKey.D2, "Remover dinheiro", () => RemoveMoney(null)))
                .Add(new MenuItem(ConsoleKey.D3, "Mostrar histórico", ShowHistory))
                .Add(new MenuItem(ConsoleKey.D4, "Sair", Finish));

            // Criando a lista de eventos
            eventsList = new Lista<Event<double>>();
        }

        public void Start()
        {
            // Iniciando o loop
            running = true;

            while (running)
            {
                // Limpando o console e mostrando o menu
                Console.Clear();
                ShowMenu();
                AnsiConsole.Markup("Digite o item do menu: ");

                // Recuperando chave do menu
                var key = Console.ReadKey();

                // Buscando o menuItem dentro da lista através da chave
                var menuItem = menuList.Find((value, index) => value.Code.Equals(key.Key));

                // Caso encontre item na lista, chama a ação configurada
                if (menuItem != null) menuItem.Action();
            }
        }

        private void ShowMenu()
        {
            // Transformando a lista em uma string e mostrando os itens
            var textMenu = menuList.Reduce((acc, value, index) => acc += $"[bold gray]{value.Code})[/] {value.Text}\n", "");
            AnsiConsole.MarkupLine(textMenu);

            // Colocando cor para o saldo e mostrando o valor
            var color = amount < 0 ? "red" : amount > 0 ? "green" : "blue";
            AnsiConsole.Markup($"Seu saldo é [bold {color}]${amount}[/].\n");
        }

        private void AddMoney(double? addAmount)
        {
            // Limpando o console
            Console.Clear();

            // Caso não tenha passado o valor via parametro, perguntar para o usuário
            if (addAmount == null) addAmount = AnsiConsole.Ask<double>("Digite o valor: ");

            // Adiciona o valor ao saldo e adiciona o evento para a lista de eventos
            amount += (double)addAmount;
            eventsList.Add(new Event<double>(EventsCode.Add, (double)addAmount));
        }

        private void RemoveMoney(double? removeAmount)
        {
            // Limpando o console
            Console.Clear();

            // Caso não tenha passado o valor via parametro, perguntar para o usuário
            if (removeAmount == null) removeAmount = AnsiConsole.Ask<double>("Digite o valor: ");

            // Remove o valor do saldo e adiciona o evento para a lista de eventos
            amount -= (double)removeAmount;
            eventsList.Add(new Event<double>(EventsCode.Remove, (double)removeAmount));
        }

        private void ShowHistory()
        {
            // Transforma a lista de eventos em string
            var text = eventsList.Reduce((acc, @event, index) =>
            {
                acc += $"[bold]{index + 1}.[/]";

                if (@event.type.Equals(EventsCode.Add)) acc += $"Adicionou valor de ${@event.value}";
                if (@event.type.Equals(EventsCode.Remove)) acc += $"Removeu valor de ${@event.value}";

                return acc += "\n";
            }, "");

            // Limpa o console e mostra os eventos
            Console.Clear();
            AnsiConsole.MarkupLine(text);

            // Pergunta para o usuário se deseja apagar um item (y/n)
            // Se não quiser ele volta para o loop
            if (!AnsiConsole.Confirm("Deseja apagar algum item?")) return;

            // Caso contrário, pergunta o usuário qual item deve ser deletado
            Event<double>? element = null;

            // Enquanto o usuário não digitar uma chave válida, a pergunta vai ser refeita.
            while (element == null)
            {
                var code = AnsiConsole.Ask<int>("Digite o código do item: ");

                // Busca o evento através do indíce
                element = eventsList.At(code - 1);

                // Caso não tenha encontrado evento, alerta o usuário
                if (element == null) AnsiConsole.MarkupLine("[red]Item não encontrado![/]");
            }

            // Se o evento foi de adição, o valor vai ser subtraido
            if (element.type.Equals(EventsCode.Add)) amount -= element.value;

            // Se o evento foi de subtração, o valor vai ser adicionado
            if (element.type.Equals(EventsCode.Remove)) amount += element.value;

            // Remove o evento da lista
            eventsList.Remove(element);
        }

        private async void Finish()
        {
            // Limpa o console e dá tchau
            Console.Clear();
            AnsiConsole.Markup("[bold]Até a próxima![/]");

            // Sai do loop
            running = false;
            await Task.Delay(10 * 1000);
        }
    }

    class Program
    {
        static void Main()
        {
            // Instanciando o programa
            var program = new Bank();

            // Iniciando o loop
            program.Start();
        }
    }
}
