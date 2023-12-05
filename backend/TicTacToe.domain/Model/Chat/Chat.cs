namespace TicTacToe.domain.Model.Chat
{
    public class GameChat
    {
        public Stack<ChatMessage> Messages { get; }

        public GameChat()
        {
            Messages = new();
        }

        public GameChat(ChatMessage message)
        {
            Messages = new();
            Messages.Push(message);
        }

        public void Add(string author, string content)
        {
            if (string.IsNullOrWhiteSpace(author)) return;
            if (string.IsNullOrWhiteSpace(content)) return;

            Messages.Push(new ChatMessage(author, content, TimeProvider.System.GetUtcNow().DateTime));
        }

        public ChatMessage Last()
        {
            return Messages.Peek();
        }
    }

    public class ChatMessage
    {
        public DateTime When { get; }

        public string Content { get; }

        public string Author { get; }

        public ChatMessage(string author, string content, DateTime when)
        {
            Author = author;
            Content = content;
            When = when;
        }
    }
}
