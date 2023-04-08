using System;

namespace ProjectAldea
{
    public interface IOptional<T>
    {
        public bool HasValue { get; }
        public T Value { get; }

        public bool HasMessage { get; }
        public string Message { get; }
        public string VerboseMessage { get; }

        public IOptional<S> FlatMap<S>(Func<T, IOptional<S>> mapper);
        public IOptional<S> Map<S>(Func<T, S> mapper);
        public string TryConsume(Func<T, string> mapper);
        public string Consume(Action<T> consumer);
    }

    internal class OptionalValue<T> : IOptional<T>
    {
        public OptionalValue(T value)
        {
            this.Value = value;
        }

        public bool HasValue { get => true; }

        public T Value { get; }

        public bool HasMessage { get => false; }
        public string Message => throw new NotSupportedException("This IOptional does not contain a message");
        public string VerboseMessage => throw new NotSupportedException("This IOptional does not contain a message");

        public string Consume(Action<T> consumer)
        {
            return String.Empty;
        }

        public IOptional<S> FlatMap<S>(Func<T, IOptional<S>> mapper)
        {
            if (mapper == null)
            {
                throw new ArgumentException("No mapping function provided");
            }

            return mapper.Invoke(this.Value);
        }

        public IOptional<S> Map<S>(Func<T, S> mapper)
        {
            if (mapper == null)
            {
                throw new ArgumentException("No mapping function provided");
            }

            return Optional.OfValue(mapper.Invoke(this.Value));
        }

        public string TryConsume(Func<T, string> mapper)
        {
            if (mapper == null)
            {
                throw new ArgumentException("No mapping function provided");
            }

            return mapper.Invoke(this.Value);
        }
    }

    internal class OptionalMessage<T> : IOptional<T>
    {
        public OptionalMessage(string message, string verboseMessage = null)
        {
            this.VerboseMessage = verboseMessage;
            this.Message = message;
        }

        public bool HasValue { get => false; }
        public T Value => throw new NotSupportedException("This IOptional does not contain a value");

        public bool HasMessage { get => true; }
        public string Message { get; }
        public string VerboseMessage { get; }

        public string Consume(Action<T> consumer)
        {
            return this.Message;
        }

        public IOptional<S> FlatMap<S>(Func<T, IOptional<S>> mapper)
        {
            return Optional.OfMessage<S>(this.Message, this.VerboseMessage);
        }

        public IOptional<S> Map<S>(Func<T, S> mapper)
        {
            return Optional.OfMessage<S>(this.Message, this.VerboseMessage);
        }

        public string TryConsume(Func<T, string> mapper)
        {
            return this.Message;
        }
    }
}
