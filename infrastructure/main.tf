terraform {
  backend "azurerm" {}
}

provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "rg-dnma-prod" {
  name     = "rg-dnma-prod"
  location = "East US"
}
