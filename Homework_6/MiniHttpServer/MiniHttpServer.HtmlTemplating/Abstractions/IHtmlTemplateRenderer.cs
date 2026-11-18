namespace MiniHttpServer.HtmlTemplating.Abstractions;

/// <summary>
/// Определяет контракт HTML-шаблонизатора для формирования HTML-разметки
/// на основе шаблона и переданной модели данных.
/// </summary>
/// <remarks>
/// Поддерживает подстановку значений свойств модели (${Property}),
/// условные конструкции ($if, $else, $endif)
/// и циклы ($foreach, $endfor).
/// </remarks>
public interface IHtmlTemplateRenderer
{
    /// <summary>
    /// Выполняет обработку HTML-шаблона, переданного в виде строки.
    /// </summary>
    /// <typeparam name="T">Тип модели данных.</typeparam>
    /// <param name="htmlTemplate">Строка с HTML-шаблоном.</param>
    /// <param name="dataModel">Объект модели данных.</param>
    /// <returns>Готовая HTML-разметка.</returns>
    string RenderFromString<T>(string htmlTemplate, T dataModel);

    /// <summary>
    /// Выполняет обработку HTML-шаблона, загруженного из файла.
    /// </summary>
    /// <typeparam name="T">Тип модели данных.</typeparam>
    /// <param name="filePath">Путь к файлу шаблона.</param>
    /// <param name="dataModel">Объект модели данных.</param>
    /// <returns>Готовая HTML-разметка.</returns>
    string RenderFromFile<T>(string filePath, T dataModel);

    /// <summary>
    /// Выполняет обработку HTML-шаблона из файла и сохраняет результат в указанный файл.
    /// </summary>
    /// <typeparam name="T">Тип модели данных.</typeparam>
    /// <param name="inputFilePath">Путь к файлу шаблона.</param>
    /// <param name="outputFilePath">Путь к выходному файлу.</param>
    /// <param name="dataModel">Объект модели данных.</param>
    /// <returns>Сгенерированная HTML-разметка.</returns>
    string RenderToFile<T>(string inputFilePath, string outputFilePath, T dataModel);
}