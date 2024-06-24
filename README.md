This program queries all AWS regions and generates XLS files containing information about the discovered objects. AWS credentials should be saved in a JSON file, and the path to this file should be passed to the program as a parameter.

The JSON file format is as follows:

{
  "AccessKeyId": "AWS_ACCESS_KEY",
  "SecretAccessKey": "AWS_SECRET_KEY"
}

wget -O AWS_Inventory https://github.com/Constantine-SRV/AWS_Inventory/releases/download/build-all-20240624200206/AWS_Inventory

chmod +x AWS_Inventory

nano aws.json

./AWS_Inventory aws.json


