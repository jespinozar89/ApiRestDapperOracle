create or replace NONEDITIONABLE PACKAGE PKG_CUSTOMERS AS
    -- Procedimiento para obtener un cliente por su ID
    PROCEDURE GetByIdCustomer(
    p_customer_id IN CUSTOMERS.CUSTOMER_ID%TYPE,
    p_cursor OUT SYS_REFCURSOR
    );

    -- Procedimiento para obtener todos los clientes
    PROCEDURE GetAllCustomers(
    p_cursor OUT SYS_REFCURSOR
    );

    PROCEDURE AddCustomer(
        p_email_address IN CUSTOMERS.EMAIL_ADDRESS%TYPE,
        p_full_name IN CUSTOMERS.FULL_NAME%TYPE,
        p_customer_id OUT CUSTOMERS.CUSTOMER_ID%TYPE
    );
    
    PROCEDURE UpdateCustomer(
        p_customer_id IN CUSTOMERS.CUSTOMER_ID%TYPE,
        p_email_address IN CUSTOMERS.EMAIL_ADDRESS%TYPE,
        p_full_name IN CUSTOMERS.FULL_NAME%TYPE,
        p_rows_updated OUT NUMBER
    );
    
    PROCEDURE DeleteCustomer(
        p_customer_id IN CUSTOMERS.CUSTOMER_ID%TYPE,
        p_rows_deleted OUT NUMBER
    );
  
END PKG_CUSTOMERS;