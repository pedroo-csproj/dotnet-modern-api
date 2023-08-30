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
  admin_enabled       = true
}

resource "azurerm_service_plan" "sp-dnma" {
  name                = var.service_plan_name
  location            = azurerm_resource_group.rg-dnma.location
  resource_group_name = azurerm_resource_group.rg-dnma.name
  os_type             = var.service_plan_os_type
  sku_name            = var.service_plan_sku_name
}

resource "azurerm_linux_web_app" "lwa-dnma" {
  name                = var.linux_web_app_name
  resource_group_name = azurerm_resource_group.rg-dnma.name
  location            = azurerm_resource_group.rg-dnma.location
  service_plan_id     = azurerm_service_plan.sp-dnma.id
  https_only          = var.linux_web_app_https_only

  site_config {
    application_stack {
      docker_registry_url      = "https://${var.linux_web_app_name}.azurecr.io"
      docker_image_name        = "${var.linux_web_app_name}:${var.build_id}"
      docker_registry_username = azurerm_container_registry.cr-dnma.admin_username
      docker_registry_password = azurerm_container_registry.cr-dnma.admin_password
    }
  }

  app_settings = {
    "DOCKER_REGISTRY_SERVER_PASSWORD" = azurerm_container_registry.cr-dnma.admin_password
    "DOCKER_REGISTRY_SERVER_URL"      = "https://${var.linux_web_app_name}.azurecr.io"
    "DOCKER_REGISTRY_SERVER_USERNAME" = azurerm_container_registry.cr-dnma.admin_username
  }
}
