using RapidPayAPI.Models;

namespace RapidPayAPI
{
    public class UFEService
    {
        private readonly RapidPayDbContext _db;
        public UFEService(RapidPayDbContext rapidPayDbContext)
        {
            this._db = rapidPayDbContext;
        }
        public decimal GetFee()
        {
            using var transaction = this._db.Database.BeginTransaction();
            try
            {
                var fee = new Fee();
                var rnd = new Random();
                var rndFactor = (rnd.Next(0, 200) / 100);

                fee = this._db.Fees.FirstOrDefault();
                if (fee != null)
                {
                    fee.Fee1 = fee.Fee1 * rndFactor;
                }
                else
                {
                    fee = new Fee() { Fee1 = rndFactor };
                    this._db.Fees.Add(fee);
                }
                this._db.SaveChanges();
                transaction.Commit();
                return fee.Fee1;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
