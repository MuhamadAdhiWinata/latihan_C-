using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace SharedKernel;

public class CodeGenerator(DbContext context)
{
    private readonly DbContext _context = context ?? throw new ArgumentNullException(nameof(context));
    
    private string _prefix = string.Empty;
    private string _dateFormat = "yyMM";
    private int _sequenceLength = 4;
    private string? _separator = null;
    
    public CodeGenerator Prefix(string prefix)
    {
        _prefix = prefix;
        return this;
    }

    public CodeGenerator DateFormat(string format)
    {
        _dateFormat = format;
        return this;
    }

    public CodeGenerator Sequence(int length)
    {
        _sequenceLength = length;
        return this;
    }

    public CodeGenerator Separator(string? separator)
    {
        _separator = separator;
        return this;
    }

    
    /// <summary>
    ///     Generates a new sequential code for the specified entity type
    /// </summary>
    /// <typeparam name="TEntity">The entity type to generate code for</typeparam>
    /// <param name="prefix">Code prefix (e.g., "INV", "PO")</param>
    /// <param name="codeColumn">Name of the code column in the entity</param>
    /// <param name="sequenceLength">Length of the sequence number with zero padding (default: 4)</param>
    /// <param name="separator">Separator character between parts (default: null for no separator)</param>
    /// <returns>Generated code string</returns>
    public async Task<string> GenerateAsync<TEntity>(
        string prefix,
        string codeColumn,
        int sequenceLength = 4,
        string? separator = null) where TEntity : class
    {
        ValidateParameters(prefix, codeColumn, sequenceLength);

        var basePattern = BuildBasePattern(prefix, separator);
        var lastCode = await FindLastCodeAsync<TEntity>(basePattern, codeColumn);
        var nextSequence = CalculateNextSequence(lastCode, basePattern);

        return FormatNewCode(basePattern, nextSequence, sequenceLength);
    }
    
    public async Task<string> GenerateAsync<TEntity>(string codeColumn)
        where TEntity : class
    {
        ValidateParameters(_prefix, codeColumn, _sequenceLength);

        var basePattern = BuildBasePattern(_prefix, _separator);
        var lastCode = await FindLastCodeAsync<TEntity>(basePattern, codeColumn);
        var nextSequence = CalculateNextSequence(lastCode, basePattern);

        return FormatNewCode(basePattern, nextSequence, _sequenceLength);
    }

    private static void ValidateParameters(string prefix, string codeColumn, int sequenceLength)
    {
        if (string.IsNullOrWhiteSpace(prefix))
            throw new ArgumentException("Prefix cannot be null or empty", nameof(prefix));

        if (string.IsNullOrWhiteSpace(codeColumn))
            throw new ArgumentException("Code column name cannot be null or empty", nameof(codeColumn));

        if (sequenceLength <= 0)
            throw new ArgumentException("Sequence length must be greater than 0", nameof(sequenceLength));
    }

    private static string BuildBasePattern(string prefix, string? separator)
    {
        var yearMonth = DateTime.UtcNow.ToString("yyMM");

        if (string.IsNullOrEmpty(separator)) return $"{prefix}{yearMonth}";

        return $"{prefix}{separator}{yearMonth}{separator}";
    }

    private async Task<string?> FindLastCodeAsync<TEntity>(string basePattern, string codeColumn)
        where TEntity : class
    {
        var entitySet = _context.Set<TEntity>();
        var filterExpression = BuildStartsWithExpression<TEntity>(basePattern, codeColumn);

        return await entitySet
            .AsNoTracking()
            .Where(filterExpression)
            .OrderByDescending(e => EF.Property<string>(e, codeColumn))
            .Select(e => EF.Property<string>(e, codeColumn))
            .FirstOrDefaultAsync();
    }

    private static Expression<Func<TEntity, bool>> BuildStartsWithExpression<TEntity>(
        string basePattern,
        string codeColumn) where TEntity : class
    {
        var parameter = Expression.Parameter(typeof(TEntity), "entity");
        var property = Expression.Property(parameter, codeColumn);
        var startsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) })!;
        var startsWithCall = Expression.Call(property, startsWithMethod, Expression.Constant(basePattern));

        return Expression.Lambda<Func<TEntity, bool>>(startsWithCall, parameter);
    }

    private static int CalculateNextSequence(string? lastCode, string basePattern)
    {
        if (string.IsNullOrWhiteSpace(lastCode))
            return 1;

        var sequencePart = lastCode.Substring(basePattern.Length);

        var digitsOnly = ExtractDigits(sequencePart);
        return int.TryParse(digitsOnly, out var sequence) ? sequence + 1 : 1;

        // if (string.IsNullOrEmpty(separator))
        // {
        //     // For separatorless codes, the sequence is just digits at the end
        //
        // }
        // else
        // {
        //     // For codes with separators, split and get the last part
        //     var parts = lastCode.Split(separator);
        //     if (parts.Length == 0)
        //         return 1;
        //
        //     var lastSequencePart = parts.Last();
        //     var digitsOnly = ExtractDigits(lastSequencePart);
        //     return int.TryParse(digitsOnly, out int sequence) ? sequence + 1 : 1;
        // }
    }

    private static string ExtractDigits(string input)
    {
        return new string(input.Where(char.IsDigit).ToArray());
    }

    private static string FormatNewCode(string basePattern, int sequence, int sequenceLength)
    {
        var paddedSequence = sequence.ToString($"D{sequenceLength}");
        return $"{basePattern}{paddedSequence}";
    }
}