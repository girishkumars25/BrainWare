-- Stored Procedure for getting order products
CREATE PROCEDURE GetOrderProducts
AS
BEGIN
    SELECT op.price, op.order_id, op.product_id, op.quantity, p.name, p.price 
    FROM orderproduct op 
    INNER JOIN product p ON op.product_id = p.product_id;
END;
