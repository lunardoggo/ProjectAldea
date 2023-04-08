using System.Collections.Generic;
using System.Linq;
using System;

namespace ProjectAldea
{
    public static class Optional
    {
        public static IOptional<T> OfValue<T>(T value)
        {
            return new OptionalValue<T>(value);
        }

        public static IOptional<T> OfMessage<T>(string message, string verboseMessage = null)
        {
            return new OptionalMessage<T>(message, verboseMessage);
        }

        public static IOptional<IEnumerable<T>> OfCollection<T>(IEnumerable<IOptional<T>> collection)
        {
            if (collection.Any(_item => _item.HasMessage))
            {
                IEnumerable<IOptional<T>> withMessage = collection.Where(_item => _item.HasMessage);

                return Optional.OfMessage<IEnumerable<T>>(String.Join(Environment.NewLine, withMessage.Select(_item => _item.Message)), String.Join(Environment.NewLine, withMessage.Select(_item => _item.VerboseMessage)));
            }

            return Optional.OfValue<IEnumerable<T>>(collection.Select(_item => _item.Value));
        }
    }
}
