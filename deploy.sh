#!/bin/bash

# Define variables
SERVER_USER="ec2-user"
SERVER_IP="16.171.137.18"
KEY_PATH="/d/ACTIVE/AWSEC2/patrick.pem"  
REMOTE_PATH="/usr/applications/ocpg"
LOCAL_PATH="bin/Release/net8.0/publish"

# Delete old build artifacts
echo "Deleting bin directory..."
rm -rf bin

Build and Test
echo "Building app..."
dotnet build

if [ $? -ne 0 ]; then
  echo "Build failed. Exiting..."
  exit 1
fi

echo "Running tests..."
dotnet test

if [ $? -ne 0 ]; then
  echo "Tests failed. Exiting..."
  exit 1
fi


echo "Deleting bin again after testing directory..."
rm -rf bin

# Publish (Release) app
echo "Publishing app..."
dotnet publish -c Release

if [ $? -ne 0 ]; then
  echo "Publish failed. Exiting..."
  exit 1
fi

# Deploy to server
echo "Deploying backend files to server..."

for file in "$LOCAL_PATH"/*; do
  if [ -f "$file" ]; then
    scp -i "$KEY_PATH" "$file" "$SERVER_USER@$SERVER_IP:$REMOTE_PATH"
  fi
done


if [ $? -eq 0 ]; then
  echo "Deployment successful!"
else
  echo "Deployment failed."
  exit 1
fi

echo "Done!"
