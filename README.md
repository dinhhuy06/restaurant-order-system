\# Hệ thống đặt món và xử lý tại bếp (Đề tài 04)



Khách hàng gửi yêu cầu đặt món. Đơn được chuyển qua hàng đợi đến bộ phận

bếp để xử lý. Khi trạng thái đơn thay đổi, khách nhận thông báo thời gian thực.



\## Kiến trúc



```

Client -> Order API -> RabbitMQ -> Kitchen Worker -> RabbitMQ -> Notification Service -> Client

&#x20;             |                          |

&#x20;             +-------> Database <-------+

```



\## Chức năng chính



1\. Xem danh sách món ăn

2\. Tạo đơn đặt món

3\. Đưa đơn vào hàng đợi xử lý của bếp

4\. Kitchen Worker nhận và xử lý đơn theo thứ tự

5\. Cập nhật trạng thái: Pending, Preparing, Ready, Cancelled

6\. Phát sự kiện khi trạng thái đơn thay đổi

7\. Gửi thông báo realtime đến client

8\. Tra cứu chi tiết và trạng thái đơn



\## API



| Method | Endpoint | Mô tả |

|---|---|---|

| GET | /menu | Danh sách món ăn |

| POST | /orders | Tạo đơn đặt món |

| GET | /orders/{id} | Chi tiết và trạng thái đơn |

| GET | /orders | Danh sách đơn |



\## Kế hoạch



| Tuần | Nội dung |

|---|---|

| 3 | Khởi tạo repo, docker-compose, thiết kế CSDL, khung project |

| 4 | GET /menu, POST /orders, GET /orders, GET /orders/{id} |

| 5 | Order API publish đơn vào RabbitMQ |

| 6 | Kitchen Worker consume, xử lý đơn, cập nhật trạng thái |

| 7 | Kitchen Worker publish event, Notification Service consume |

| 8 | Đẩy thông báo realtime (WebSocket/SSE), client đơn giản |

| 9 | Xử lý lỗi, test, README, báo cáo, demo |



\## Cách chạy



(Sẽ cập nhật)

