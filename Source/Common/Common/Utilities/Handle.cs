using System;
using System.Threading.Tasks;
using Andead.Chat.Common.Logging;

namespace Andead.Chat.Common.Utilities
{
    public static class Handle
    {
        private const string ErrorFormatString = "Error{0}: {1}";
        private const string ErrorDescriptionFormatString = " while {0}";
        private const string RepeatingFormatString = "Repeating {0}";
        private const string MaxLimitReachedFormatString = "Max limit of tries reached during {0}.";

        public static ILogger Logger { get; set; }

        public static void Errors(Action method, string description)
        {
            Errors(method, description, null);
        }

        public static T Errors<T>(Func<T> method, string description)
        {
            return Errors(method, description, null);
        }

        public static void Errors(Action method, string description, Action<Exception> handler)
        {
            Func<object> methodFunc = () =>
            {
                method();
                return null;
            };

            Errors(methodFunc, description, handler);
        }

        public static T Errors<T>(Func<T> method, string description, Action<Exception> handler)
        {
            try
            {
                return method();
            }
            catch (Exception e)
            {
                Logger.Warn(string.Format(ErrorFormatString, description != null
                        ? string.Format(ErrorDescriptionFormatString, description)
                        : null, e),
                    WarnCategory.UnexpectedBehavior);

                handler?.Invoke(e);
                return default(T);
            }
        }

        public static void Errors(Action method, int maxTimes, string description, Action<Exception> handler)
        {
            Errors<object>(() =>
            {
                method();
                return null;
            }, maxTimes, description, handler);
        }

        public static T Errors<T>(Func<T> method, int maxTimes, string description, Action<Exception> handler)
        {
            Logger.Info(string.Format(RepeatingFormatString, description), InfoCategory.Workflow);

            var count = 0;
            var success = false;
            do
            {
                T result = Errors(() =>
                {
                    T got = method();

                    success = true;
                    return got;
                }, description, e =>
                {
                    count++;
                    handler?.Invoke(e);
                });

                if (success)
                {
                    return result;
                }
            } while (!success && (count < maxTimes));

            Logger.Info(string.Format(MaxLimitReachedFormatString, description), InfoCategory.Workflow);
            return default(T);
        }

        public static Task ErrorsAsync(Func<Task> method, string description)
        {
            return ErrorsAsync(method, description, (Action<Exception>) null);
        }

        public static Task<T> ErrorsAsync<T>(Func<Task<T>> method, string description)
        {
            return ErrorsAsync(method, description, (Action<Exception>) null);
        }

        public static async Task ErrorsAsync(Func<Task> method, string description, Action<Exception> handler)
        {
            await ErrorsAsync<object>(async () =>
            {
                await method();
                return null;
            }, description, handler);
        }

        public static async Task ErrorsAsync(Func<Task> method, string description, Func<Exception, Task> handler)
        {
            await ErrorsAsync<object>(async () =>
            {
                await method();
                return null;
            }, description, handler);
        }

        public static async Task<T> ErrorsAsync<T>(Func<Task<T>> method, string description, Action<Exception> handler)
        {
            try
            {
                return await method();
            }
            catch (Exception e)
            {
                Logger.Warn(string.Format(ErrorFormatString, description != null
                        ? string.Format(ErrorDescriptionFormatString, description)
                        : null, e),
                    WarnCategory.UnexpectedBehavior);

                handler?.Invoke(e);
                return default(T);
            }
        }

        public static async Task<T> ErrorsAsync<T>(Func<Task<T>> method, string description, Func<Exception, Task> handler)
        {
            try
            {
                return await method();
            }
            catch (Exception e)
            {
                Logger.Warn(string.Format(ErrorFormatString, description != null
                        ? string.Format(ErrorDescriptionFormatString, description)
                        : null, e),
                    WarnCategory.UnexpectedBehavior);

                if (handler != null)
                {
                    await ErrorsAsync(async () => await handler(e), description);
                }

                return default(T);
            }
        }

        public static async Task ErrorsAsync(Func<Task> method, int maxTimes, string description,
            Action<Exception> handler)
        {
            await ErrorsAsync<object>(async () =>
            {
                await method();
                return null;
            }, maxTimes, description, handler);
        }

        public static async Task ErrorsAsync(Func<Task> method, int maxTimes, string description,
            Func<Exception, Task> handler)
        {
            await ErrorsAsync<object>(async () =>
            {
                await method();
                return null;
            }, maxTimes, description, handler);
        }

        public static async Task<T> ErrorsAsync<T>(Func<Task<T>> method, int maxTimes, string description,
            Action<Exception> handler)
        {
            Logger.Info(string.Format(RepeatingFormatString, description), InfoCategory.Workflow);

            var count = 0;
            var success = false;
            do
            {
                T result = await ErrorsAsync(async () =>
                {
                    T got = await method();

                    success = true;
                    return got;
                }, description, e =>
                {
                    count++;
                    handler?.Invoke(e);
                });

                if (success)
                {
                    return result;
                }
            } while (!success && (count < maxTimes));

            Logger.Info(string.Format(MaxLimitReachedFormatString, description), InfoCategory.Workflow);
            return default(T);
        }

        public static async Task<T> ErrorsAsync<T>(Func<Task<T>> method, int maxTimes, string description,
            Func<Exception, Task> handler)
        {
            Logger.Info($"Repeating {description}", InfoCategory.Workflow);

            var count = 0;
            var success = false;
            do
            {
                T result = await ErrorsAsync(async () =>
                {
                    T got = await method();

                    success = true;
                    return got;
                }, description, async e =>
                {
                    count++;

                    if (handler != null)
                    {
                        await handler(e);
                    }
                });

                if (success)
                {
                    return result;
                }
            } while (!success && (count < maxTimes));

            Logger.Info($"Max limit of tries reached during {description}.", InfoCategory.Workflow);
            return default(T);
        }
    }
}