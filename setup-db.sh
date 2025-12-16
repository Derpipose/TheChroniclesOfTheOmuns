#!/bin/bash

# Chronicles of the Omuns - Database Setup Script (Bash)
# This script creates migrations, applies them, and runs the app
# For use with WSL (Windows Subsystem for Linux) or Git Bash

echo "=== Chronicles of the Omuns - Database Setup ==="

# Step 1: Navigate to PlayerApp directory
echo -e "\nStep 1: Navigating to PlayerApp directory..."
PLAYER_APP_PATH="/mnt/c/Users/thefl/source/repos/TheChroniclesOfTheOmuns/PlayerApp"

if [ ! -d "$PLAYER_APP_PATH" ]; then
    echo "✗ Error: PlayerApp directory not found at $PLAYER_APP_PATH"
    echo "Please update the path in this script if your project location is different."
    exit 1
fi

cd "$PLAYER_APP_PATH"
echo "✓ Current directory: $(pwd)"

# Step 2: Create migration
echo -e "\nStep 2: Creating database migration..."
dotnet ef migrations add InitialCreate

if [ $? -eq 0 ]; then
    echo "✓ Migration created successfully"
else
    echo "⚠ Migration may already exist or there was an issue"
fi

# Step 3: Apply migration
echo -e "\nStep 3: Applying migration to database..."
dotnet ef database update

if [ $? -eq 0 ]; then
    echo "✓ Database updated successfully"
else
    echo "✗ Error updating database"
    exit 1
fi

# Step 4: Run the app
echo -e "\nStep 4: Starting the application..."
dotnet run

echo -e "\n=== Setup Complete ==="
