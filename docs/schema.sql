CREATE TABLE menu_items (
    id           SERIAL PRIMARY KEY,
    name         VARCHAR(200) NOT NULL,
    price        NUMERIC(10,2) NOT NULL CHECK (price >= 0),
    is_available BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE orders (
    id            SERIAL PRIMARY KEY,
    customer_name VARCHAR(200) NOT NULL,
    status        VARCHAR(20) NOT NULL DEFAULT 'Pending'
        CHECK (status IN ('Pending','Preparing','Ready','Cancelled')),
    created_at    TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at    TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE order_items (
    id           SERIAL PRIMARY KEY,
    order_id     INT NOT NULL REFERENCES orders(id),
    menu_item_id INT NOT NULL REFERENCES menu_items(id),
    quantity     INT NOT NULL CHECK (quantity > 0)
);

CREATE INDEX idx_orders_status_created ON orders(status, created_at);

INSERT INTO menu_items (name, price) VALUES
    ('Phở bò', 55000),
    ('Bún chả', 50000),
    ('Cơm tấm sườn', 45000),
    ('Trà đá', 5000),
    ('Cà phê sữa', 25000);