import psycopg2

# Database connection parameters
DB_HOST = "localhost"      # because you expose 5432:5432 in docker-compose
DB_PORT = 5432
DB_NAME = "backend_db"   # your ${DB_DATABASE}
DB_USER = "postgres"             # your ${DB_USER}
DB_PASSWORD = "postgres"  # your ${DB_PASSWORD}

# SQL commands
SQL_COMMANDS = """
INSERT INTO player (player_id, username, gold)
VALUES ('D465D2FD-092C-40A6-945D-E29E99FA524A', 'koch', 100);

INSERT INTO unlocked_character (id, player_id, character_id, level)
VALUES (1, 'D465D2FD-092C-40A6-945D-E29E99FA524A', 'char001', 1);
"""

def main():
    try:
        # Connect to PostgreSQL
        conn = psycopg2.connect(
            host=DB_HOST,
            port=DB_PORT,
            dbname=DB_NAME,
            user=DB_USER,
            password=DB_PASSWORD
        )

        # Create a cursor
        cur = conn.cursor()

        # Execute the SQL commands
        cur.execute(SQL_COMMANDS)

        # Commit the transaction
        conn.commit()

        print("Data inserted successfully!")

        # Close the connection
        cur.close()
        conn.close()

    except Exception as e:
        print(f"Error: {e}")

if __name__ == "__main__":
    main()
