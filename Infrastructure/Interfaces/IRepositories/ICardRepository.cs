using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCPG.Infrastructure.Interfaces.IRepositories
{
    public interface ICardRepository
    {
        Task<bool> CreateCard(string adviceReference, string token);
        Task<Core.Models.Entities.Cards> GetCardByAdviceReference(string parameter);
    }
}