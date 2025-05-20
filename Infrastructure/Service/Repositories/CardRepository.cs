using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CentralPG.Data;
using Microsoft.EntityFrameworkCore;
using OCPG.Infrastructure.Interfaces.IRepositories;

namespace OCPG.Infrastructure.Service.Repositories
{
    public class CardRepository : ICardRepository
    {
        private readonly DataBaseContext dataBaseContext;

        public CardRepository(DataBaseContext dataBaseContext)
        {
            this.dataBaseContext = dataBaseContext;
        }


        public async Task<bool> CreateCard(string adviceReference, string token)
        {
            try
            {
               
                var existingCards = await dataBaseContext.cards
                    .Where(c => c.adviceReference == adviceReference)
                    .ToListAsync();

                if (existingCards.Any())
                {
                    dataBaseContext.cards.RemoveRange(existingCards);
                    await dataBaseContext.SaveChangesAsync();
                }

                // Create and add the new card
                var newCard = new Core.Models.Entities.Cards
                {
                    adviceReference = adviceReference,
                    token = token
                };
                await dataBaseContext.cards.AddAsync(newCard);
                await dataBaseContext.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }



        public async Task<Core.Models.Entities.Cards> GetCardByAdviceReference(string parameter)
        {
            var gottenCard = await dataBaseContext.cards.FirstOrDefaultAsync(c => c.adviceReference == parameter);
            return gottenCard;
        }

    }
}