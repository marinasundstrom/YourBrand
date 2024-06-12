namespace ChatApp.Domain;

public static class Errors
{
    public static class Channels
    {
        public static readonly Error ChannelNotFound = new Error(nameof(ChannelNotFound), "Channel not found", string.Empty);

        public static readonly Error ChannelWithNameAlreadyExists = new Error(nameof(ChannelWithNameAlreadyExists), "Channel with name already exists", string.Empty);
    }

    public static class Messages
    {
        public static readonly Error MessageNotFound = new Error(nameof(MessageNotFound), "Message not found", string.Empty);

        public static readonly Error NotAllowedToDelete = new Error(nameof(NotAllowedToDelete), "Not allowed to delete", string.Empty);

        public static readonly Error NotAllowedToEdit = new Error(nameof(NotAllowedToEdit), "Not allowed to edit", string.Empty);
    }

    public static class Users
    {
        public static readonly Error UserNotFound = new Error(nameof(UserNotFound), "User not found", string.Empty);
    }
}
