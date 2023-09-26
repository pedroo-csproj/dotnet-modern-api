using DotNETModernAPI.Infrastructure.CrossCutting.Core.Enums;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;

namespace DotNETModernAPI.Infrastructure.CrossCutting.Core.Models;

public class ResultWrapper<TEntity>
{
    public ResultWrapper(EErrorCode errorCode) =>
        ErrorCode = errorCode;

    public ResultWrapper(TEntity data)
    {
        ErrorCode = EErrorCode.NoError;
        Data = data;
    }

    public ResultWrapper(IList<ValidationFailure> validationFailures)
    {
        ErrorCode = EErrorCode.InvalidEntity;
        Errors = ConvertErrors(validationFailures);
    }

    public ResultWrapper(IEnumerable<IdentityError> identityErrors)
    {
        ErrorCode = EErrorCode.IdentityError;
        Errors = ConvertErrors(identityErrors);
    }

    public ResultWrapper(EErrorCode errorCode, IList<string> errors)
    {
        ErrorCode = errorCode;
        Errors = errors;
    }

    public bool Success { get => ErrorCode == EErrorCode.NoError; }
    public EErrorCode ErrorCode { get; private set; }
    public IList<string> Errors { get; private set; } = new List<string>();
    public TEntity Data { get; set; }

    private static IList<string> ConvertErrors(IList<ValidationFailure> validationFailures)
    {
        var errors = new List<string>();

        if (validationFailures.Any())
            errors = validationFailures.Select(vf => vf.ErrorMessage).ToList();

        return errors;
    }

    private static IList<string> ConvertErrors(IEnumerable<IdentityError> identityErrors)
    {
        var errors = new List<string>();

        if (identityErrors.Any())
            errors = identityErrors.Select(ie => ie.Description).ToList();

        return errors;
    }
}

public class ResultWrapper
{
    public ResultWrapper() =>
        ErrorCode = EErrorCode.NoError;

    public ResultWrapper(EErrorCode errorCode) =>
        ErrorCode = errorCode;

    public ResultWrapper(IList<ValidationFailure> validationFailures)
    {
        ErrorCode = EErrorCode.InvalidEntity;
        Errors = ConvertErrors(validationFailures);
    }

    public ResultWrapper(IEnumerable<IdentityError> identityErrors)
    {
        ErrorCode = EErrorCode.IdentityError;
        Errors = ConvertErrors(identityErrors);
    }

    public ResultWrapper(EErrorCode errorCode, IList<string> errors)
    {
        ErrorCode = errorCode;
        Errors = errors;
    }

    public bool Success { get => ErrorCode == EErrorCode.NoError; }
    public EErrorCode ErrorCode { get; private set; }
    public IList<string> Errors { get; private set; } = new List<string>();

    private static IList<string> ConvertErrors(IList<ValidationFailure> validationFailures)
    {
        var errors = new List<string>();

        if (validationFailures.Any())
            errors = validationFailures.Select(vf => vf.ErrorMessage).ToList();

        return errors;
    }

    private static IList<string> ConvertErrors(IEnumerable<IdentityError> identityErrors)
    {
        var errors = new List<string>();

        if (identityErrors.Any())
            errors = identityErrors.Select(ie => ie.Description).ToList();

        return errors;
    }
}
