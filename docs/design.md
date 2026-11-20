\# Thiết kế



\## Luồng trạng thái



Pending -> Preparing -> Ready

Pending -> Cancelled

Preparing -> Cancelled



Ready và Cancelled là trạng thái cuối, không đổi tiếp.



\## Hàng đợi RabbitMQ



1\. Queue `order.created`

&#x20;  - Order API publish sau khi lưu đơn vào DB

&#x20;  - Kitchen Worker consume (prefetch = 1, xử lý lần lượt theo thứ tự vào)

&#x20;  - Nội dung: { orderId }



2\. Exchange `order.events` (topic), routing key `order.status.changed`

&#x20;  - Kitchen Worker publish khi đổi trạng thái

&#x20;  - Notification Service consume

&#x20;  - Nội dung: { orderId, status, timestamp }



\## Quy tắc



\- Lưu DB trước, publish sau.

\- Kitchen Worker chỉ ack message sau khi xử lý xong.

