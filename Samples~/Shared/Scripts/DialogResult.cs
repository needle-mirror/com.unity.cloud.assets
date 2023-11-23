namespace Unity.Cloud.Assets.Samples
{
    public class Result<T> : Result, IDialogResult<T>
    {
        internal Result(bool confirmed, T content) : base(confirmed)
        {
            Content = content;
        }

        public T Content { get; }
    }

    public class Result
    {
        protected Result(bool confirmed)
        {
            IsConfirmed = confirmed;
        }

        public bool IsConfirmed { get; private set; }

        public static IDialogResult<T> Cancelled<T>()
        {
            return new Result<T>(false, default);
        }

        public static Result Cancelled()
        {
            return new Result(false);
        }

        public static Result Confirmed()
        {
            return new Result(true);
        }

        public static IDialogResult<T> From<T>(T content)
        {
            return new Result<T>(true, content);
        }

        public static IDialogResult<T> From<T>(Result<T> result)
        {
            return new Result<T>(result.IsConfirmed, result.Content);
        }
    }
}
