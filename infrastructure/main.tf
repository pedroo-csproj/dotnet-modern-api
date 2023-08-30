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

resource "azurerm_container_registry" "cr-dnma" {
  name                = var.container_registry_name
  resource_group_name = azurerm_resource_group.rg-dnma.name
  location            = azurerm_resource_group.rg-dnma.location
  sku                 = var.container_registry_sku
}
