# OrderService

## Project Setup

1. Clone the repository.
4. Run the application by: `docker-compose up  --build`.
5. The API will be available at `http://localhost:8080/swagger`.

## API Endpoints

### 1. POST /order
- Create a new order.

**Request body:**
{
    "productName": "Some Product",
    "customerName": "Abhishek"
}
