using Application.Abstraction;
using Domain.Entities;

namespace Application.Abstractions
{
    /// <summary>
    /// Interface for managing <see cref="RecoveryQuestion"/> entities within the data layer.
    /// This interface is documented by AI.
    /// </summary>
    public interface IRecoveryQuestionRepository : IEfRepository<RecoveryQuestion>
    {
    }
}
