using Application.Dtos.RecoveryQuestion;
using Domain.Entities;

namespace Application.Mappers
{
    public static class RecoveryQuestionMapper
    {
        public static RecoveryQuestion ToEntity(CreateRecoveryQuestionRequest dto)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            return new RecoveryQuestion
            {
                Question = dto.Question.Trim(),
            };
        }

        public static void ApplyUpdate(RecoveryQuestion entity, UpdateRecoveryQuestionRequest dto)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            if (!string.IsNullOrWhiteSpace(dto.Question)) entity.Question = dto.Question.Trim();
        }

        public static RecoveryQuestionResponse ToResponse(RecoveryQuestion entity)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));

            return new RecoveryQuestionResponse
            {
                Id = entity.Id,
                Question = entity.Question
            };
        }

        public static List<RecoveryQuestionResponse> ToResponseList(IEnumerable<RecoveryQuestion> entities)
        {
            if (entities is null) throw new ArgumentNullException(nameof(entities));

            return entities.Select(ToResponse).ToList();
        }
    }
}
