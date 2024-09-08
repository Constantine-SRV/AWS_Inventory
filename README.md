
# AWS Inventory Tool

This program queries all AWS regions and generates XLS files containing information about the discovered objects. AWS credentials should be saved in a JSON file, and the path to this file should be passed to the program as a parameter.

### AWS Credentials JSON Format

The JSON file format should be as follows:

```json
{
  "AccessKeyId": "AWS_ACCESS_KEY",
  "SecretAccessKey": "AWS_SECRET_KEY"
}
```

## Linux 

```bash
wget -O AWS_Inventory_linux https://github.com/Constantine-SRV/AWS_Inventory/releases/download/latest_release/AWS_Inventory_linux
chmod +x AWS_Inventory_linux
nano aws.json
./AWS_Inventory_linux aws.json
```

## MacOS 

```bash
curl -L -o AWS_Inventory_mac https://github.com/Constantine-SRV/AWS_Inventory/releases/download/latest_release/AWS_Inventory_mac
chmod +x AWS_Inventory_mac
nano aws.json
./AWS_Inventory_mac aws.json
```

## Windows

Download the Windows binary from the following link: https://github.com/Constantine-SRV/AWS_Inventory/releases/download/latest_release/AWS_Inventory_win.exe

Create an `aws.json` file with the AWS credentials in the same directory as the executable:

```bash
AWS_Inventory_win.exe aws.json
```

## How the program works

### Step 1: Region availability check
The first step checks the availability of AWS regions. The name of each region will be displayed along with messages for any regions that are not available.

Example output:
```bash
Region checking: ap-northeast-1
```

The result of the region check will be saved to a file. If the program is rerun within 24 hours, the data will be loaded from this file to avoid unnecessary checks.

### Step 2: Data collection
The second step collects data from all available regions and saves the results in XLS files. For easier analysis on multiple screens, separate files are created for each type of object. As a result, 19 files named after the corresponding AWS objects will be generated in the current directory.
