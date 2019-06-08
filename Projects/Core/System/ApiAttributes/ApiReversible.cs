namespace System
{
    /// <summary>
    /// Обозначает метод, действие которого отменяется путем отката транзакции или UnitOfWork. Кроме того, при получении данных игнорируется текущий контекст транзакции или UnitOfWork. 
    /// Это означает, что метод можно вызывать в рамках транзакции, если были затронуты те же таблицы, что используются в методе.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ApiReversible : Attribute
    {
    }
}
