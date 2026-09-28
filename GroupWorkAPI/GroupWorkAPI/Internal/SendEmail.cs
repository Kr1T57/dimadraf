using System.Text;
using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using GroupWorkAPI.Model;

namespace GroupWorkAPI.Internal
{
    internal static class SendEmail
    {
        public static async Task SendReceipt(PostgresContext context, Order order)
        {
            var fullOrder = await context.Orders
                .AsNoTracking()
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
                        .ThenInclude(pv => pv.Product)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
                        .ThenInclude(pv => pv.Color)
                .FirstOrDefaultAsync(o => o.Id == order.Id);

            if (fullOrder == null || fullOrder.User == null)
            {
                throw new Exception("Заказ или пользователь не найдены для отправки чека.");
            }

            var itemsHtml = new StringBuilder();
            decimal totalProductsPrice = 0;

            foreach (var item in fullOrder.OrderItems)
            {
                decimal itemPrice = item.PriceAtPurchase;
                decimal itemSum = itemPrice * item.Quantity;
                totalProductsPrice += itemSum;

                itemsHtml.Append($@"
                    <tr style=""border-bottom: 1px dashed #cbd5e1;"">
                        <td style=""padding: 8px 0; text-align: left; vertical-align: top;"">
                            <div style=""font-weight: 900; font-size: 14px; text-transform: uppercase; font-family: sans-serif; color: #000000; line-height: 1.25;"">
                                {item.ProductVariant.Product.Name}
                            </div>
                            <div style=""font-size: 11px; color: #64748b; font-weight: bold; text-transform: uppercase; font-family: sans-serif; margin-top: 2px;"">
                                Цвет: {item.ProductVariant.Color.Name} / Размер: {item.ProductVariant.Size}
                            </div>
                            <div style=""font-size: 11px; font-weight: 500; color: #94a3b8; font-family: sans-serif; margin-top: 4px;"">
                                {itemPrice} ₽ × {item.Quantity} шт.
                            </div>
                        </td>
                        <td style=""padding: 8px 0; text-align: right; vertical-align: top; font-weight: 900; font-size: 14px; font-family: sans-serif; color: #000000; white-space: nowrap; width: 100px;"">
                            {itemSum} ₽
                        </td>
                    </tr>");
            }

            string htmlBody = $@"
<div style=""max-w: 448px; margin: 0 auto; background-color: #ffffff; padding: 24px; border: 4px solid #000000; font-family: sans-serif; color: #000000;"">

  <div style=""text-align: center; border-bottom: 4px solid #000000; padding-bottom: 16px; margin-bottom: 24px;"">
    <h1 style=""font-size: 36px; font-weight: 900; letter-spacing: -0.05em; text-transform: uppercase; font-style: italic; color: rgb(0,30,98); margin: 0; font-family: sans-serif;"">
      asics
    </h1>
    <p style=""font-size: 12px; text-transform: uppercase; font-weight: bold; letter-spacing: 0.1em; color: #64748b; margin: 4px 0 0 0; font-family: sans-serif;"">
      Anima Sana In Corpore Sano
    </p>
    <div style=""margin-top: 16px; background-color: #000000; color: #ffffff; font-size: 12px; font-weight: 900; text-transform: uppercase; padding: 4px 12px; display: inline-block; letter-spacing: 0.05em; font-family: sans-serif;"">
      Товарный чек / Спецификация
    </div>
  </div>

  <table style=""width: 100%; border-collapse: collapse; margin-bottom: 16px; border-bottom: 2px solid #000000; padding-bottom: 16px;"">
    <tr>
      <td style=""padding-bottom: 8px; font-size: 12px; font-weight: bold; text-transform: uppercase; font-family: sans-serif;"">
        <span style=""color: #64748b; display: block; font-size: 11px;"">Номер заказа:</span>
        <span style=""font-size: 14px; font-weight: 900;"">#ASICS-{fullOrder.Id}</span>
      </td>
      <td style=""padding-bottom: 8px; text-align: right; font-size: 12px; font-weight: bold; text-transform: uppercase; font-family: sans-serif;"">
        <span style=""color: #64748b; display: block; font-size: 11px;"">Дата и время:</span>
        <span style=""font-size: 14px; font-weight: 900;"">{fullOrder.OrderDate:dd.05.yyyy HH:mm}</span>
      </td>
    </tr>
    <tr>
      <td style=""padding-top: 8px; padding-bottom: 16px; font-size: 12px; font-weight: bold; text-transform: uppercase; font-family: sans-serif;"">
        <span style=""color: #64748b; display: block; font-size: 11px;"">Покупатель:</span>
        <span style=""font-weight: 900;"">{fullOrder.User.Name}</span>
      </td>
      <td style=""padding-top: 8px; padding-bottom: 16px; text-align: right; font-size: 12px; font-weight: bold; text-transform: uppercase; font-family: sans-serif;"">
        <span style=""color: #64748b; display: block; font-size: 11px;"">Тип доставки:</span>
        <span style=""font-weight: 900; color: #16a34a;"">Примерка (0 ₽)</span>
      </td>
    </tr>
  </table>

  <div style=""margin-bottom: 24px;"">
    <h3 style=""font-weight: 900; text-transform: uppercase; font-size: 14px; font-style: italic; margin-bottom: 12px; color: #334155; font-family: sans-serif;"">
        Состав заказа:
    </h3>

    <table style=""width: 100%; border-collapse: collapse;"">
      {itemsHtml}
    </table>
  </div>

  <div style=""border-top: 2px solid #000000; background-color: #f8fafc; padding: 12px; border: 1px solid #e2e8f0;"">
    <table style=""width: 100%; border-collapse: collapse; font-size: 12px; text-transform: uppercase; font-weight: bold; color: #475569; font-family: sans-serif;"">
      <tr>
        <td style=""padding: 2px 0; text-align: left;"">Стоимость товаров:</td>
        <td style=""padding: 2px 0; text-align: right; color: #000000;"">{totalProductsPrice} ₽</td>
      </tr>
      <tr>
        <td style=""padding: 2px 0; text-align: left;"">Доставка и примерка:</td>
        <td style=""padding: 2px 0; text-align: right; color: #16a34a;"">бесплатно</td>
      </tr>
      <tr>
        <td style=""padding: 2px 0; text-align: left; color: #dc2626;"">Скидка по промокоду:</td>
        <td style=""padding: 2px 0; text-align: right; color: #dc2626;"">-0 ₽</td>
      </tr>
    </table>

    <table style=""width: 100%; border-collapse: collapse; border-top: 4px solid #000000; margin-top: 12px; padding-top: 12px; font-family: sans-serif;"">
      <tr>
        <td style=""padding-top: 12px; font-size: 20px; font-weight: 900; text-transform: uppercase; letter-spacing: -0.05em; text-align: left; color: #000000;"">
            Итого к оплате:
        </td>
        <td style=""padding-top: 12px; font-size: 24px; font-weight: 900; font-style: italic; color: rgb(0,30,98); text-align: right; white-space: nowrap;"">
            {fullOrder.TotalAmount} ₽
        </td>
      </tr>
    </table>
  </div>

  <div style=""text-align: center; margin-top: 32px; border-top: 1px solid #e2e8f0; padding-top: 16px;"">
    <p style=""font-size: 10px; text-transform: uppercase; font-weight: 900; letter-spacing: -0.05em; color: #475569; margin: 0; font-family: sans-serif;"">
      Спасибо за покупку в ASICS Store!
    </p>
    <p style=""font-size: 9px; color: #94a3b8; font-weight: 500; margin-top: 4px; margin-bottom: 0; font-family: sans-serif;"">
      Оригинальный товар сертифицирован. Возврат в течение 14 дней.
    </p>
  </div>
</div>";

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("rafaelguccigang@gmail.com", "ewes mtoi dswt xrvh"),
                EnableSsl = true,
                UseDefaultCredentials = false
            };

            MailAddress from = new("rafaelguccigang@gmail.com", "ASICS Store");
            MailAddress to = new(fullOrder.User.Email);

            var mailMessage = new MailMessage(from, to)
            {
                Subject = $"Кассовый чек по заказу #ASICS-{fullOrder.Id}",
                Body = htmlBody,
                IsBodyHtml = true,
            };

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}