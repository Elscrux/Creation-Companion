using System.Reflection;
namespace CreationEditor.Services.Config;

public static class ConfigValuesExtensions {
    extension(ConfigValues configValues) {
        /// <summary>
        /// Gets the value of a config option, throwing an exception if the option is optional and has no value set.
        /// </summary>
        /// <param name="configOption">The config option to get the value of</param>
        /// <returns>The value of the config option, or its default value if it is not optional and has no value set</returns>
        /// <exception cref="ArgumentException">Thrown if the config option is optional and has no value set</exception>
        public object Get(IConfigOption configOption) {
            if (configValues.TryGetValue(configOption.Name, out var value)) return value;

            if (configOption.IsOptional) {
                throw new ArgumentException($"Config option '{configOption.Name}' is optional and has no value set - use TryGet() to avoid this exception");
            }

            return configOption.DefaultObjectValue;
        }

        /// <summary>
        /// Gets the value of a typed config option, converting it to <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to convert the value to</typeparam>
        /// <param name="configOption">The config option to get the value of</param>
        /// <returns>The value of the config option converted to <typeparamref name="T"/></returns>
        /// <exception cref="ArgumentException">Thrown if the config option is optional and has no value set</exception>
        public T Get<T>(IConfigOption<T> configOption) => configValues.Get(configOption as IConfigOption).ConvertTo<T>();

        /// <summary>
        /// Gets the value of a config option, converting it to <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to convert the value to</typeparam>
        /// <param name="configOption">The config option to get the value of</param>
        /// <returns>The value of the config option converted to <typeparamref name="T"/></returns>
        /// <exception cref="ArgumentException">Thrown if the config option is optional and has no value set</exception>
        public T Get<T>(IConfigOption configOption) => configValues.Get(configOption).ConvertTo<T>();

        /// <summary>
        /// Tries to get the value of a config option, returning <see langword="null"/> if the option is optional and has no value set.
        /// </summary>
        /// <param name="configOption">The config option to get the value of</param>
        /// <returns>The value of the config option, its default value if it is not optional and has no value set, or <see langword="null"/> if it is optional and has no value set</returns>
        public object? TryGet(IConfigOption configOption) {
            if (configValues.TryGetValue(configOption.Name, out var value)) return value;
            if (configOption.IsOptional) return null;

            return configOption.DefaultObjectValue;
        }

        /// <summary>
        /// Tries to get the value of a config option defined as a static property on a config definition, converting it to <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to convert the value to</typeparam>
        /// <param name="configDefinition">The config definition that declares the static property, or <see langword="null"/></param>
        /// <param name="staticPropertyName">The name of the static property that holds the <see cref="IConfigOption"/></param>
        /// <returns>The value of the config option converted to <typeparamref name="T"/>, its default value, or <see langword="default"/> if the option cannot be resolved</returns>
        public T? TryGet<T>(IConfigDefinition? configDefinition, string staticPropertyName) {
            var type = configDefinition?.GetType();

            if (type?
                .GetProperty(staticPropertyName, BindingFlags.Static | BindingFlags.Public)?
                .GetValue(null) is IConfigOption configOption) {

                if (configValues.TryGetValue(configOption.Name, out var value)) {
                    return value.ConvertTo<T>();
                }

                var defaultValue = configOption.GetType()
                    .GetProperty(nameof(IConfigOption.DefaultObjectValue), BindingFlags.Static | BindingFlags.Public)?
                    .GetValue(null);

                if (defaultValue is not null) {
                    return defaultValue.ConvertTo<T>();
                }
            }

            return default;
        }

        /// <summary>
        /// Tries to get the value of a config option by name, converting it to <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The reference type to convert the value to</typeparam>
        /// <param name="configOptionName">The name of the config option</param>
        /// <returns>The value of the config option converted to <typeparamref name="T"/>, or <see langword="null"/> if no value is set</returns>
        public T? TryGet<T>(string configOptionName) where T : class {
            var value = configValues.GetValueOrDefault(configOptionName);
            return value?.ConvertTo<T>();
        }

        /// <summary>
        /// Tries to get the value of a typed config option, converting it to <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The reference type to convert the value to</typeparam>
        /// <param name="configOption">The config option to get the value of</param>
        /// <returns>The value of the config option converted to <typeparamref name="T"/>, or <see langword="null"/> if no value is set</returns>
        public T? TryGet<T>(IConfigOption<T> configOption) where T : class {
            var value = configValues.TryGet(configOption as IConfigOption);
            if (value is null) return null;

            return value.ConvertTo<T>();
        }

        /// <summary>
        /// Tries to get the value of a config option, converting it to <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The reference type to convert the value to</typeparam>
        /// <param name="configOption">The config option to get the value of</param>
        /// <returns>The value of the config option converted to <typeparamref name="T"/>, or <see langword="null"/> if no value is set</returns>
        public T? TryGet<T>(IConfigOption configOption) where T : class {
            var value = configValues.TryGet(configOption);
            if (value is null) return null;

            return value.ConvertTo<T>();
        }

        /// <summary>
        /// Tries to get the value of a typed config option, converting it to <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The struct type to convert the value to</typeparam>
        /// <param name="configOption">The config option to get the value of</param>
        /// <returns>The value of the config option converted to <typeparamref name="T"/>, or <see langword="null"/> if no value is set</returns>
        public T? TryGetStruct<T>(IConfigOption<T> configOption) where T : struct {
            var value = configValues.TryGet(configOption);
            if (value is null) return null;

            return value.ConvertTo<T>();
        }

        /// <summary>
        /// Tries to get the value of a config option, converting it to <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The struct type to convert the value to</typeparam>
        /// <param name="configOption">The config option to get the value of</param>
        /// <returns>The value of the config option converted to <typeparamref name="T"/>, or <see langword="null"/> if no value is set</returns>
        public T? TryGetStruct<T>(IConfigOption configOption) where T : struct {
            var value = configValues.TryGet(configOption);
            if (value is null) return null;

            return value.ConvertTo<T>();
        }

        /// <summary>
        /// Determines whether a value is set for the given config option.
        /// </summary>
        /// <param name="configOption">The config option to check</param>
        /// <returns><see langword="true"/> if a value is set for the config option, otherwise <see langword="false"/></returns>
        public bool Has(IConfigOption configOption) => configValues.ContainsKey(configOption.Name);

        /// <summary>
        /// Sets the value of a typed config option.
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="configOption">The config option to set the value of</param>
        /// <param name="value">The value to set</param>
        public void Set<T>(IConfigOption<T> configOption, T value) => configValues[configOption.Name] = value!;

        /// <summary>
        /// Sets the value of a config option by name.
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="configOptionName">The name of the config option</param>
        /// <param name="value">The value to set</param>
        public void Set<T>(string configOptionName, T value) => configValues[configOptionName] = value!;

        /// <summary>
        /// Sets the value of a config option defined as a static property on a config definition.
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="configDefinition">The config definition that declares the static property, or <see langword="null"/></param>
        /// <param name="staticPropertyName">The name of the static property that holds the config option name</param>
        /// <param name="value">The value to set</param>
        public void Set<T>(IConfigDefinition? configDefinition, string staticPropertyName, T value) {
            if (configDefinition?
                .GetType()?
                .GetProperty(staticPropertyName, BindingFlags.Static | BindingFlags.Public)?
                .GetValue(null) is IConfigOption configOption) {
                configValues[configOption.Name] = value!;
            }
        }
    }

    /// <summary>
    /// Converts a boxed value to <typeparamref name="T"/>, handling numeric and enum conversions
    /// that a direct unboxing cast would reject (e.g. a boxed <see cref="long"/> to an enum type).
    /// </summary>
    private static T ConvertTo<T>(this object value) {
        if (value is T typed) return typed;

        var targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
        if (targetType.IsEnum) {
            return (T) Enum.ToObject(targetType, value);
        }

        return (T) Convert.ChangeType(value, targetType);
    }
}
