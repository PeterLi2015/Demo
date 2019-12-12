using XDropsWater.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace XDropsWater.Bll
{
    public class Common
    {
        public static int GetRoleID(string roleXmlString, decimal orderQuantity, int currentOrderId, out decimal currentQuantity)
        {
            if (!string.IsNullOrWhiteSpace(roleXmlString))
            {
                XDocument roleXml = XDocument.Parse(roleXmlString);
                int roleID = 0;
                int quantity = 0;
                foreach (XElement element in roleXml.XPathSelectElements("//Role"))
                {
                    roleID = int.Parse(element.XPathSelectElement("./RoleID").Value);
                    quantity = int.Parse(element.XPathSelectElement("./Quantity").Value);
                    if (currentOrderId < roleID)
                    {
                        if (orderQuantity >= quantity)
                        {
                            currentQuantity = orderQuantity - quantity;
                            return roleID;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            currentQuantity = 0;
            return 0;
        }

        /// <summary>
        /// 获取角色
        /// </summary>
        /// <param name="roleXmlString"></param>
        /// <param name="roleAndOrderAmount"></param>
        /// <param name="unitAmount"></param>
        /// <param name="minusAmount"></param>
        /// <param name="currentRoleId"></param>
        /// <param name="roleAmount"></param>
        /// <returns></returns>
        public static int GetRoleID1(string roleXmlString, decimal roleAndOrderAmount, decimal unitAmount, decimal minusAmount, int currentRoleId, out decimal roleAmount)
        {
            if (!string.IsNullOrWhiteSpace(roleXmlString))
            {
                XDocument roleXml = XDocument.Parse(roleXmlString);
                int roleID = 0;
                int quantity = 0;
                decimal upgradeAmount = 0m;
                foreach (XElement element in roleXml.XPathSelectElements("//Role"))
                {
                    roleID = int.Parse(element.XPathSelectElement("./RoleID").Value);
                    quantity = int.Parse(element.XPathSelectElement("./Quantity").Value);
                    if (currentRoleId < roleID)
                    {
                        // 升级金额 = 升级数量 * 单位金额 - 优惠金额
                        upgradeAmount = quantity * unitAmount - minusAmount;

                        // 角色金额 + 订单金额 >= 升级金额
                        if (roleAndOrderAmount >= upgradeAmount)
                        {
                            // 角色金额 = 原来的角色金额 + 订单金额 - 升级金额
                            roleAmount = roleAndOrderAmount - upgradeAmount;
                            return roleID;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            // 如果匹配不到，角色金额=角色金额+订单金额，角色ID=当前角色ID
            roleAmount = roleAndOrderAmount;
            return currentRoleId;
        }

        public static List<CityAgent> GetCityAgent(string xml)
        {
            List<CityAgent> result = new List<CityAgent>();
            CityAgent model = null;
            if (!string.IsNullOrWhiteSpace(xml))
            {
                XDocument roleXml = XDocument.Parse(xml);
                foreach (XElement element in roleXml.XPathSelectElements("//CityAgent"))
                {
                    model = new CityAgent();
                    model.Count = int.Parse(element.XPathSelectElement("./Count").Value);
                    model.Minus = int.Parse(element.XPathSelectElement("./Minus").Value);
                    result.Add(model);
                }
            }
            return result;
        }

        public static int GetRiseQuantity(string roleXmlString, int currentRoleQuantity)
        {
            if (!string.IsNullOrWhiteSpace(roleXmlString))
            {
                XDocument roleXml = XDocument.Parse(roleXmlString);
                var lastElement = roleXml.XPathSelectElements("//Role").LastOrDefault();
                int roleID = int.Parse(lastElement.XPathSelectElement("./RoleID").Value);
                int tempRoleID = 0;
                int quantity = 0;
                foreach (XElement element in roleXml.XPathSelectElements("//Role"))
                {
                    tempRoleID = int.Parse(element.XPathSelectElement("./RoleID").Value);
                    if (tempRoleID == roleID + 1)
                    {
                        quantity = int.Parse(element.XPathSelectElement("./Quantity").Value);
                        break;
                    }
                }
                return quantity - currentRoleQuantity;
            }
            return GlobalConstants.LowRiseQuantity;
        }

        /// <summary>
        /// 获取升级金额
        /// </summary>
        /// <param name="roleXmlString"></param>
        /// <param name="unitAmount"></param>
        /// <param name="rolePrice"></param>
        /// <param name="currentRoleAmount"></param>
        /// <returns></returns>
        public static decimal GetUpgradeAmount(int roleID, string roleXmlString, decimal unitAmount, decimal currentRoleAmount)
        {
            decimal upgradeAmount = 0m;
            if (!string.IsNullOrWhiteSpace(roleXmlString))
            {
                XDocument roleXml = XDocument.Parse(roleXmlString);
                var lastElement = roleXml.XPathSelectElements("//Role").LastOrDefault();
                //int roleID = int.Parse(lastElement.XPathSelectElement("./RoleID").Value);
                int tempRoleID = 0;

                // 升级数量
                int upgradeQuantity = 0;
                foreach (XElement element in roleXml.XPathSelectElements("//Role"))
                {
                    tempRoleID = int.Parse(element.XPathSelectElement("./RoleID").Value);
                    if (tempRoleID == roleID)
                    {
                        upgradeQuantity = int.Parse(element.XPathSelectElement("./UpgradeQuantity").Value);
                        break;
                    }
                }
                // 升级金额 = 升级数量 * 单位金额 - 当前角色金额
                upgradeAmount = upgradeQuantity * unitAmount - currentRoleAmount;

            }
            return upgradeAmount;
        }

        
        public static string GetTotalAmountDescription(int price, string roleXmlString, int orderQuantity, int currentRoleID, int currentQuantity, out int totalAmount)
        {
            totalAmount = 0;
            string sResult = string.Empty;
            if (string.IsNullOrWhiteSpace(roleXmlString))
            {
                totalAmount = price * orderQuantity;
                sResult += string.Format(" {0}X{1} ", price.ToString(), orderQuantity.ToString());
                sResult += string.Format(" = {0}", totalAmount.ToString());
                return sResult;
            }
            if (!string.IsNullOrWhiteSpace(roleXmlString))
            {
                XDocument roleXml = XDocument.Parse(roleXmlString);
                int upgradeRoleID = 0;
                int upgradeQuantity = 0;
                int totalQuantity = orderQuantity + currentQuantity;
                int currentPrice = 0;
                int counter = 0;
                int quantity = 0;
                
                foreach (XElement element in roleXml.XPathSelectElements("//Role"))
                {
                    upgradeRoleID = int.Parse(element.XPathSelectElement("./RoleID").Value);
                    quantity = int.Parse(element.XPathSelectElement("./Quantity").Value);
                    upgradeQuantity = int.Parse(element.XPathSelectElement("./UpgradeQuantity").Value);
                    currentPrice = int.Parse(element.XPathSelectElement("./CurrentPrice").Value);
                    //if (counter == 0)
                    //{
                    //    if (currentRoleID + 1 == upgradeRoleID)
                    //    {
                    //        sResult += string.Format(" {0}X{1} ", currentPrice.ToString(), orderQuantity.ToString());
                    //        totalAmount += currentPrice * orderQuantity;
                    //        counter++;
                    //        break;
                    //    }
                    //}
                    if (totalQuantity >= quantity)
                    {
                        //if(currentRoleID<upgradeRoleID)
                        //{
                        //    if (counter == 0)
                        //    {
                        //        sResult += string.Format(" {0}X{1} ", currentPrice.ToString(), (totalQuantity - quantity).ToString());
                        //        totalAmount += currentPrice * (totalQuantity - upgradeQuantity);
                        //        counter++;
                        //    }
                        //    else
                        //    {
                        //        sResult += string.Format(" + {0}X{1} ", currentPrice.ToString(), upgradeQuantity.ToString());
                        //        totalAmount += currentPrice * (totalQuantity - upgradeQuantity);
                        //        counter++;
                        //    }
                        //}
                        //else if (currentRoleID >= upgradeRoleID)
                        //{
                        //    if (counter == 0)
                        //    {
                        //        sResult += string.Format(" {0}X{1} ", currentPrice.ToString(), (orderQuantity - currentQuantity).ToString());
                        //        totalAmount += currentPrice * (orderQuantity - currentQuantity);
                        //        counter++;
                        //    }
                        //    else
                        //    {
                        //        sResult += string.Format(" + {0}X{1} ", currentPrice.ToString(), (orderQuantity - currentQuantity).ToString());
                        //        totalAmount += currentPrice * (orderQuantity - currentQuantity);
                        //        counter++;
                        //    }
                        //    break;
                        //}

                        if (counter == 0)
                        {
                            if (currentRoleID < upgradeRoleID)
                            {
                                
                                if (totalQuantity > quantity)
                                {
                                    sResult += string.Format(" {0}X{1} ", currentPrice.ToString(), (totalQuantity - quantity).ToString());
                                    totalAmount += currentPrice * (totalQuantity - quantity);
                                    counter++;
                                }
                            }
                            else
                            {
                                sResult += string.Format(" {0}X{1} ", currentPrice.ToString(), orderQuantity.ToString());
                                totalAmount += currentPrice * orderQuantity;
                                counter++;
                                break;
                            }
                           
                        }
                        else
                        {
                            if (currentRoleID < upgradeRoleID)
                            {
                                sResult += string.Format(" + {0}X{1} ", currentPrice.ToString(), upgradeQuantity.ToString());
                                totalAmount += currentPrice * upgradeQuantity;
                                counter++;
                            }
                            else
                            {
                                sResult += string.Format(" + {0}X{1} ", currentPrice.ToString(), (upgradeQuantity - currentQuantity).ToString());
                                totalAmount += currentPrice * (upgradeQuantity - currentQuantity);
                                counter++;
                            }
                        }
                    }
                }
                sResult += string.Format(" = {0}",totalAmount.ToString());
            }
            return sResult;
        }

        public static string GetTotalAmountDescription1(int price, string roleXmlString, decimal orderQuantity, int currentRoleID, decimal currentQuantity, out decimal totalAmount)
        {
            totalAmount = 0;
            string sResult = string.Empty;
            //总代和董事
            if (string.IsNullOrWhiteSpace(roleXmlString))
            {
                totalAmount = price * orderQuantity;
                totalAmount = decimal.Round(totalAmount, 2);
                sResult += string.Format(" {0}X{1} ", price.ToString(), orderQuantity.ToString());
                sResult += string.Format(" = {0}", totalAmount.ToString());
                return sResult;
            }

            if (!string.IsNullOrWhiteSpace(roleXmlString))
            {
                XDocument roleXml = XDocument.Parse(roleXmlString);
                int upgradeRoleID = 0;
                int upgradeQuantity = 0;

                decimal totalQuantity = orderQuantity + currentQuantity;
                int currentPrice = 0;
                int counter = 0;
                int quantity = 0;

                foreach (XElement element in roleXml.XPathSelectElements("//Role"))
                {
                    upgradeRoleID = int.Parse(element.XPathSelectElement("./RoleID").Value);
                    quantity = int.Parse(element.XPathSelectElement("./Quantity").Value);
                    upgradeQuantity = int.Parse(element.XPathSelectElement("./UpgradeQuantity").Value);
                    currentPrice = int.Parse(element.XPathSelectElement("./CurrentPrice").Value);
                    
                    //总数量大于升级数量
                    if (totalQuantity >= quantity)
                    {
                        if (counter == 0)
                        {
                            if (currentRoleID < upgradeRoleID)
                            {

                                if (totalQuantity > quantity)
                                {
                                    sResult += string.Format(" {0}X{1} ", currentPrice.ToString(), (totalQuantity - quantity).ToString());
                                    totalAmount += currentPrice * (totalQuantity - quantity);
                                    counter++;
                                }
                            }
                            else
                            {
                                sResult += string.Format(" {0}X{1} ", currentPrice.ToString(), orderQuantity.ToString());
                                totalAmount += currentPrice * orderQuantity;
                                counter++;
                                break;
                            }

                        }
                        else
                        {
                            if (currentRoleID < upgradeRoleID)
                            {
                                sResult += string.Format(" + {0}X{1} ", currentPrice.ToString(), upgradeQuantity.ToString());
                                totalAmount += currentPrice * upgradeQuantity;
                                counter++;
                            }
                            else
                            {
                                sResult += string.Format(" + {0}X{1} ", currentPrice.ToString(), (upgradeQuantity - currentQuantity).ToString());
                                totalAmount += currentPrice * (upgradeQuantity - currentQuantity);
                                counter++;
                            }
                        }
                    }
                }
                totalAmount = decimal.Round(totalAmount, 2);
                sResult += string.Format(" = {0}", totalAmount.ToString());
            }
            return sResult;
        }

    }
}
