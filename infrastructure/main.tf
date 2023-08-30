terraform {
  backend "azurerm" {}
}

provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "rg-dnma" {
  name     = var.resource_group_name
  location = var.resource_group_location
}
