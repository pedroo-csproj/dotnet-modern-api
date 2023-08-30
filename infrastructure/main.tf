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

resource "azurerm_service_plan" "sp-dnma" {
  name                = var.service_plan_name
  location            = azurerm_resource_group.rg-dnma.location
  resource_group_name = azurerm_resource_group.rg-dnma.name
  kind                = var.service_plan_kind
  os_type             = var.service_plan_os_type
  sku_name            = var.service_plan_sku_name
  reserved            = true
}

resource "azurerm_app_service" "as-dnma" {
  name                = var.app_service_name
  resource_group_name = azurerm_resource_group.rg-dnma.name
  location            = azurerm_resource_group.rg-dnma.location
  app_service_plan_id = azurerm_service_plan.sp-dnma.id
  https_only          = var.app_service_https_only
}
