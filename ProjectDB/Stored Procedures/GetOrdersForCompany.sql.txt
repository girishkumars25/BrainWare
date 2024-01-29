-- Stored Procedure for getting orders by company id
CREATE PROCEDURE GetOrdersForCompany
    @CompanyId INT
AS
BEGIN
    SELECT c.name, o.description, o.order_id 
    FROM company c 
    INNER JOIN [order] o ON c.company_id = o.company_id
    WHERE c.company_id = @CompanyId;
END;

