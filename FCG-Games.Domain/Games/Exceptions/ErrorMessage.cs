namespace FCG_Games.Domain.Games.Exceptions
{
    public class ErrorMessage
    {
        public static GameErrorMessages Game { get; } = new();
    }

    public class GameErrorMessages
    {
        public string TitleNullOrEmpty { get; } = "O título do jogo não pode ser nulo ou vazio";
        public string NegativePrice { get; } = "O preço do jogo não pode ser negativo";
        public string LaunchYearInFuture { get; } = "A data de lançamento do jogo não pode ser no futuro";
        public string GenderRequired { get; } = "O jogo deve possuir um gênero";
        public string DeveloperRequired { get; } = "A desenvolvedora não pode ser nula ou vazia";
    }
}
